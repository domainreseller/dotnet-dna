using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Xunit;

namespace DomainNameApi.Tests
{
    public class TelemetryTests
    {
        private const string Dsn = "https://publickey@sentry.example.test/42";

        private static (DomainNameApiClient Client, FakeTransport Transport, List<(string Url, string Body, string Auth)> Sent) Make(string dsn = Dsn)
        {
            var t = new FakeTransport();
            var c = new DomainNameApiClient("00000000-0000-4000-8000-000000000001", "secret-key", false, t) { ZoneMinInterval = TimeSpan.Zero };
            var sent = new List<(string, string, string)>();
            c.TelemetryDsn = dsn;
            c.TelemetrySender = (u, b, a) => sent.Add((u, b, a));
            return (c, t, sent);
        }

        [Fact]
        public void DefaultDsnIsTheDotnetDnaSentryProject()
        {
            Assert.Equal("https://b98d4a4f7be210aa3d95b0065c5ee0cf@sentry.atakdomain.com/20", DomainNameApiClient.DefaultSentryDsn);
            using (var c = new DomainNameApiClient("r", "k", false, new FakeTransport()))
            {
                Assert.Equal(DomainNameApiClient.DefaultSentryDsn, c.TelemetryDsn);
            }
        }

        [Fact]
        public void TestRunNeverPostsToTheRealSentryProject()
        {
            // Installed by TelemetryTestGuard before any test runs.
            Assert.Same(TelemetryTestGuard.DropEvent, DomainNameApiClient.DefaultTelemetrySender);
            using (var c = new DomainNameApiClient("r", "k", false, new FakeTransport()))
            {
                Assert.Same(TelemetryTestGuard.DropEvent, c.TelemetrySender);
            }
        }

        [Fact]
        public void DefaultDsnBuildsEventsForTheDotnetDnaProject()
        {
            var (c, t, sent) = Make(DomainNameApiClient.DefaultSentryDsn);
            t.Enqueue(500, "{\"error\":{\"message\":\"boom\"}}");
            c.GetDetails("example.com");
            Assert.Single(sent);
            Assert.Equal("https://sentry.atakdomain.com/api/20/store/", sent[0].Url);
            Assert.Equal("Sentry sentry_version=7, sentry_client=dotnet-dna/1.0.1, sentry_key=b98d4a4f7be210aa3d95b0065c5ee0cf", sent[0].Auth);
        }

        [Fact]
        public void EmptyDsnIsANoOp()
        {
            var (c, t, sent) = Make("");
            t.Enqueue(500, "{\"error\":{\"message\":\"boom\"}}");
            c.GetDetails("example.com");
            Assert.Empty(sent);
        }

        [Fact]
        public void ErrorEventIsTaggedAndCredentialsAreMasked()
        {
            var (c, t, sent) = Make();
            t.Enqueue(500, "{\"error\":{\"message\":\"Registry exploded at 10.1.2.3 for 'abc'\"},\"authCode\":\"Ab1!\"}");
            c.CheckTransfer("example.com", "Ab1!xyZ9");

            Assert.Single(sent);
            var (url, body, auth) = sent[0];
            Assert.Equal("https://sentry.example.test/api/42/store/", url);
            Assert.Equal("Sentry sentry_version=7, sentry_client=dotnet-dna/1.0.1, sentry_key=publickey", auth);

            var evt = JsonNode.Parse(body);
            Assert.Equal("DomainNameAPI-DotNet", (string)evt["tags"]["application"]);
            Assert.Equal("1.0.1", (string)evt["tags"]["library_version"]);
            Assert.Equal("1.0.1", (string)evt["release"]);
            Assert.Equal("production", (string)evt["environment"]);
            Assert.Equal("00000000-0000-4000-8000-000000000001", (string)evt["tags"]["reseller"]);
            Assert.Equal("example.com", (string)evt["tags"]["domain"]);
            Assert.Equal("request:POST domains/transfers/check", (string)evt["tags"]["operation"]);
            Assert.Equal("API_500", (string)evt["tags"]["api_code"]);
            Assert.Equal("10.1.2.3", (string)evt["tags"]["ip_value"]);
            Assert.Equal("API_500: Registry exploded at {ip} for {v}", (string)evt["message"]["formatted"]);

            // Credentials masked, everything else kept.
            Assert.Equal("***", (string)evt["extra"]["request_data"]["payload"]["authCode"]);
            Assert.Equal("example.com", (string)evt["extra"]["request_data"]["payload"]["domainName"]);
            Assert.Equal("***", (string)evt["extra"]["response_data"]["authCode"]);
            Assert.DoesNotContain("Ab1!xyZ9", body);
            Assert.DoesNotContain("secret-key", body);
        }

        [Fact]
        public void IgnoredErrorsAreNotSent()
        {
            var (c, t, sent) = Make();
            t.Enqueue(404, "{\"error\":{\"message\":\"The domain name you requested could not be found.\"}}");
            c.GetDetails("missing.com");
            Assert.Empty(sent);
        }

        [Fact]
        public void DisabledReportingSendsNothing()
        {
            var (c, t, sent) = Make();
            c.SetErrorReportingEnabled(false);
            t.Enqueue(500, "{\"error\":{\"message\":\"boom\"}}");
            c.GetDetails("example.com");
            Assert.Empty(sent);
        }

        [Fact]
        public void MinimalModeDropsRequestDataDomainAndAddress()
        {
            var (c, t, sent) = Make();
            c.SetTelemetryMinimal(true);
            t.Enqueue(500, "{\"error\":{\"message\":\"boom\"}}");
            c.GetDetails("example.com");

            var evt = JsonNode.Parse(sent[0].Body);
            Assert.Null(evt["tags"]["domain"]);
            Assert.Null(evt["tags"]["ip_address"]);
            Assert.Null(evt["tags"]["url"]);
            Assert.Null(evt["tags"]["vhost_user"]);
            Assert.Equal("GET", (string)evt["extra"]["request_data"]["method"]);
            Assert.Equal("/api/v1/domains/info", (string)evt["extra"]["request_data"]["endpoint"]);
            Assert.Equal(500, (int)evt["extra"]["response_data"]["http_code"]);
            Assert.DoesNotContain("example.com", sent[0].Body);
        }

        [Fact]
        public void TelemetryFailuresNeverReachTheCaller()
        {
            var (c, t, _) = Make();
            c.TelemetrySender = (u, b, a) => throw new InvalidOperationException("sentry down");
            t.Enqueue(500, "{\"error\":{\"message\":\"boom\"}}");
            var r = c.GetDetails("example.com");
            Assert.Equal("ERROR", (string)r["result"]);
        }

        [Fact]
        public void MaskHandlesNestingListsAndEmptyValues()
        {
            var data = JsonNode.Parse("{\"apiKey\":\"k\",\"api_key\":\"\",\"Password\":null,\"nested\":{\"eppCode\":\"x\",\"list\":[{\"token\":\"t\"}]},\"keep\":\"v\"}");
            var masked = DomainNameApiClient.TelemetryMask(data);
            Assert.Equal("{\"apiKey\":\"***\",\"api_key\":\"\",\"Password\":null,\"nested\":{\"eppCode\":\"***\",\"list\":[{\"token\":\"***\"}]},\"keep\":\"v\"}",
                masked.ToJsonString());
        }

        [Fact]
        public void PerformanceEventCarriesSpanData()
        {
            var (c, t, _) = Make();
            t.Enqueue(200, "{\"resellerId\":\"r\"}");
            c.GetResellerDetails();
            var evt = c.BuildPerformanceEvent("request:GET deposit/accounts/me", 12.5, DateTime.UtcNow.AddMilliseconds(-12), DateTime.UtcNow);
            Assert.NotNull(evt);
            var json = JsonNode.Parse(evt.Value.Body);
            Assert.Equal("transaction", (string)json["type"]);
            Assert.Equal("API.request:GET deposit/accounts/me", (string)json["transaction"]);
            Assert.Equal("GET https://api.domainresellerapi.com", (string)json["spans"][0]["description"]);
            Assert.Equal(200, (int)json["spans"][0]["data"]["http.response.status_code"]);
            Assert.Equal("DomainNameAPI-DotNet", (string)json["tags"]["application"]);
        }
    }
}
