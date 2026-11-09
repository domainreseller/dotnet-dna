using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Xunit;

namespace DomainNameApi.Tests
{
    /// <summary>
    /// Replays every php-dna conformance fixture (testdata/conformance/*.json, copied
    /// unchanged from php-dna/conformance/fixtures) against the C# client with a fake
    /// transport, and compares the requests sent and the value returned.
    /// </summary>
    public class ConformanceTests
    {
        private const string Reseller = "00000000-0000-4000-8000-000000000001";
        private const string ApiKey = "conformance-api-key";
        private const string BaseUrl = "https://api.domainresellerapi.com/api/v1/";

        private static string FixtureDir => Path.Combine(AppContext.BaseDirectory, "testdata", "conformance");

        public static IEnumerable<object[]> Fixtures() =>
            Directory.GetFiles(FixtureDir, "*.json").Select(Path.GetFileName).OrderBy(f => f, StringComparer.Ordinal).Select(f => new object[] { f });

        [Fact]
        public void AllFixturesArePresent()
        {
            Assert.Equal(134, Directory.GetFiles(FixtureDir, "*.json").Length);
        }

        [Theory]
        [MemberData(nameof(Fixtures))]
        public void Fixture(string file)
        {
            var fixture = JsonNode.Parse(File.ReadAllText(Path.Combine(FixtureDir, file))).AsObject();
            var transport = new FakeTransport();
            foreach (var r in fixture["responses"]?.AsArray() ?? new JsonArray())
            {
                transport.Enqueue(r["status"].GetValue<int>(), r["body"]?.GetValue<string>() ?? "");
            }

            var client = new DomainNameApiClient(Reseller, ApiKey, false, transport) { ZoneMinInterval = TimeSpan.Zero };
            client.SetErrorReportingEnabled(false);

            var call = fixture["call"].GetValue<string>();
            var args = fixture["args"].AsArray().Select(Json.ToClr).ToList();
            var output = Json.RoundTrip((JsonNode)Invoke(client, call, args));

            // Requests: same number, same order, same method/path/query/body/auth headers.
            var expectedRequests = fixture["requests"].AsArray();
            Assert.True(expectedRequests.Count == transport.Requests.Count,
                $"{file}: expected {expectedRequests.Count} request(s), sent {transport.Requests.Count}");
            for (var i = 0; i < expectedRequests.Count; i++)
            {
                var exp = expectedRequests[i];
                var act = transport.Requests[i];
                Assert.Equal(exp["method"].GetValue<string>(), act.Method);

                Assert.StartsWith(BaseUrl, act.Url);
                var rel = act.Url.Substring(BaseUrl.Length);
                var path = rel.Split('?')[0];
                Assert.Equal(exp["path"].GetValue<string>(), path);

                AssertJson(file + " request[" + i + "].query", exp["query"], Json.ParseQuery(act.Url));
                AssertJson(file + " request[" + i + "].body", exp["body"], act.Body == null ? null : JsonNode.Parse(act.Body));

                Assert.Equal(exp["headers"]["X-API-KEY"]?.GetValue<string>(), act.Headers["X-API-KEY"]);
                Assert.Equal(exp["headers"]["__reseller"]?.GetValue<string>(), act.Headers["__reseller"]);
            }

            // Output: compare after removing date-dependent paths, which must be non-negative integers.
            var expected = fixture["output"]?.DeepClone();
            foreach (var p in fixture["ignore"]?.AsArray().Select(n => n.GetValue<string>()) ?? Enumerable.Empty<string>())
            {
                var actualValue = Detach(output, p);
                Detach(expected, p);
                Assert.True(actualValue is JsonValue v && v.TryGetValue<long>(out var days) && days >= 0,
                    $"{file}: {p} must be a non-negative integer, got {actualValue?.ToJsonString() ?? "null"}");
            }
            AssertJson(file + " output", expected, output);
        }

        private static void AssertJson(string label, JsonNode expected, JsonNode actual)
        {
            Assert.True(Json.Equal(actual, expected, out var where),
                $"{label} differs at {where}\nexpected: {expected?.ToJsonString() ?? "null"}\nactual:   {actual?.ToJsonString() ?? "null"}");
        }

        /// <summary>Remove the node at a dot path and return it.</summary>
        private static JsonNode Detach(JsonNode root, string path)
        {
            var parts = path.Split('.');
            var cur = root;
            for (var i = 0; i < parts.Length - 1; i++)
            {
                cur = cur is JsonObject o ? o[parts[i]] : cur is JsonArray a ? a[int.Parse(parts[i])] : null;
                if (cur == null) return null;
            }
            var last = parts[parts.Length - 1];
            if (cur is JsonObject obj && obj.TryGetPropertyValue(last, out var value))
            {
                obj.Remove(last);
                return value;
            }
            return null;
        }

        // ------------------------------------------------------------------
        // php-dna call name + positional args → C# method
        // ------------------------------------------------------------------

        private static object Invoke(DomainNameApiClient c, string call, List<object> a)
        {
            object Arg(int i) => i < a.Count ? a[i] : null;
            bool Has(int i) => i < a.Count;
            string S(int i) => Arg(i) == null ? null : Convert.ToString(Arg(i), System.Globalization.CultureInfo.InvariantCulture);
            int I(int i) => Convert.ToInt32(Arg(i), System.Globalization.CultureInfo.InvariantCulture);
            bool B(int i) => Convert.ToBoolean(Arg(i), System.Globalization.CultureInfo.InvariantCulture);
            int? NI(int i) => Arg(i) == null ? (int?)null : I(i);
            IDictionary<string, object> Map(int i) => Arg(i) switch
            {
                IDictionary<string, object> d => d,
                IList<object> l when l.Count == 0 => new Dictionary<string, object>(),
                null => null,
                _ => throw new ArgumentException("argument " + i + " is not a map"),
            };
            IEnumerable<string> Strings(int i) => ((IEnumerable<object>)Arg(i)).Select(o => Convert.ToString(o, System.Globalization.CultureInfo.InvariantCulture));

            switch (call)
            {
                case "getResellerDetails": return c.GetResellerDetails();
                case "getCurrentBalance": return Has(0) ? c.GetCurrentBalance(S(0)) : c.GetCurrentBalance();
                case "checkAvailability": return c.CheckAvailability(Strings(0), Strings(1), I(2), Has(3) ? S(3) : "create");
                case "getList": return Has(0) ? c.GetList(Map(0)) : c.GetList();
                case "getTldList": return Has(0) ? c.GetTldList(I(0)) : c.GetTldList();
                case "getDetails": return c.GetDetails(S(0));
                case "syncFromRegistry": return c.SyncFromRegistry(S(0));
                case "getContacts": return c.GetContacts(S(0));
                case "saveContacts": return c.SaveContacts(S(0), Map(1));
                case "modifyNameServer": return c.ModifyNameServer(S(0), Arg(1));
                case "enableTheftProtectionLock": return c.EnableTheftProtectionLock(S(0));
                case "disableTheftProtectionLock": return c.DisableTheftProtectionLock(S(0));
                case "modifyPrivacyProtectionStatus":
                    if (Arg(1) is string flag) return Has(2) ? c.ModifyPrivacyProtectionStatus(S(0), flag, S(2)) : c.ModifyPrivacyProtectionStatus(S(0), flag);
                    return Has(2) ? c.ModifyPrivacyProtectionStatus(S(0), B(1), S(2)) : c.ModifyPrivacyProtectionStatus(S(0), B(1));
                case "renew": return c.Renew(S(0), I(1));
                case "registerWithContactInfo":
                    return c.RegisterWithContactInfo(S(0), I(1), Map(2),
                        Has(3) ? Arg(3) : null,
                        Has(4) ? B(4) : true,
                        Has(5) ? B(5) : false,
                        Has(6) ? Map(6) : null);
                case "addChildNameServer": return c.AddChildNameServer(S(0), S(1), S(2));
                case "deleteChildNameServer": return c.DeleteChildNameServer(S(0), S(1));
                case "modifyChildNameServer": return c.ModifyChildNameServer(S(0), S(1), S(2));
                // A 4th (contacts) argument is ignored: transfers never send contacts.
                case "transfer": return c.Transfer(S(0), S(1), I(2));
                case "cancelTransfer": return c.CancelTransfer(S(0));
                case "approveTransfer": return c.ApproveTransfer(S(0));
                case "rejectTransfer": return c.RejectTransfer(S(0));
                case "checkTransfer": return c.CheckTransfer(S(0), S(1));
                case "getResourceRecords": return c.GetResourceRecords(S(0));
                case "addResourceRecord":
                    return c.AddResourceRecord(S(0), S(1), S(2), Arg(3), Has(4) ? I(4) : 3600, Has(5) ? B(5) : true);
                case "editResourceRecord":
                    return c.EditResourceRecord(S(0), S(1), S(2), S(3), Arg(4), Has(5) ? I(5) : 3600, Has(6) ? B(6) : true);
                case "deleteResourceRecord":
                    return c.DeleteResourceRecord(S(0), S(1), S(2), S(3), Has(4) ? B(4) : true);
                case "applyResourceRecords": return c.ApplyResourceRecords(S(0));
                case "setResourceRecordSet":
                    return c.SetResourceRecordSet(S(0), S(1), S(2), Strings(3), Has(4) ? I(4) : 3600, Has(5) ? B(5) : true);
                case "deleteResourceRecordSet":
                    return c.DeleteResourceRecordSet(S(0), S(1), S(2), Has(3) ? B(3) : true, Has(4) ? S(4) : "");
                case "addResourceRecordValue":
                    return c.AddResourceRecordValue(S(0), S(1), S(2), S(3), Has(4) ? NI(4) : null, Has(5) ? B(5) : true);
                case "removeResourceRecordValue":
                    return c.RemoveResourceRecordValue(S(0), S(1), S(2), S(3), Has(4) ? B(4) : true);
                case "replaceResourceRecordValue":
                    return c.ReplaceResourceRecordValue(S(0), S(1), S(2), S(3), S(4), Has(5) ? NI(5) : null, Has(6) ? B(6) : true);
                case "getForwarding": return c.GetForwarding(S(0));
                case "setForwarding": return Has(2) ? c.SetForwarding(S(0), S(1), S(2)) : c.SetForwarding(S(0), S(1));
                case "deleteForwarding": return c.DeleteForwarding(S(0));
                case "isTrTLD": return JsonValue.Create(c.IsTrTLD(S(0)));
                case "validateContact": return c.ValidateContact(Map(0));
                default: throw new NotSupportedException("Unmapped call " + call);
            }
        }
    }
}
