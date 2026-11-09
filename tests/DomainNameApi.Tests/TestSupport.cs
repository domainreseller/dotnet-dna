using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DomainNameApi.Tests
{
    /// <summary>Transport that serves queued responses and records requests. No network.</summary>
    internal sealed class FakeTransport : IHttpTransport
    {
        private readonly Queue<Func<HttpRequestData, HttpResponseData>> _queue = new Queue<Func<HttpRequestData, HttpResponseData>>();

        public List<HttpRequestData> Requests { get; } = new List<HttpRequestData>();

        public FakeTransport Enqueue(int status, string body)
        {
            _queue.Enqueue(_ => new HttpResponseData { StatusCode = status, Body = body ?? "", ContentType = "application/json" });
            return this;
        }

        public FakeTransport EnqueueJson(int status, object body) => Enqueue(status, JsonSerializer.Serialize(body));

        public FakeTransport EnqueueFailure(TransportException ex)
        {
            _queue.Enqueue(_ => throw ex);
            return this;
        }

        public int Remaining => _queue.Count;

        public HttpResponseData Send(HttpRequestData request)
        {
            Requests.Add(request);
            if (_queue.Count == 0)
            {
                throw new InvalidOperationException("FakeTransport: no queued response for " + request.Method + " " + request.Url);
            }
            return _queue.Dequeue()(request);
        }
    }

    internal static class Json
    {
        /// <summary>Fixture comparison rules: empty list == empty object, numbers by value, key order ignored.</summary>
        public static bool Equal(JsonNode a, JsonNode b, out string where, string path = "$")
        {
            where = null;
            if (a == null && b == null) return true;
            if (a == null || b == null)
            {
                where = path + ": " + Show(a) + " != " + Show(b);
                return false;
            }
            if (IsEmptyContainer(a) && IsEmptyContainer(b)) return true;

            if (a is JsonObject oa && b is JsonObject ob)
            {
                var keys = oa.Select(kv => kv.Key).Union(ob.Select(kv => kv.Key)).ToList();
                foreach (var k in keys)
                {
                    var hasA = oa.TryGetPropertyValue(k, out var va);
                    var hasB = ob.TryGetPropertyValue(k, out var vb);
                    if (hasA != hasB)
                    {
                        where = path + "." + k + ": present in " + (hasA ? "actual" : "expected") + " only";
                        return false;
                    }
                    if (!Equal(va, vb, out where, path + "." + k)) return false;
                }
                return true;
            }
            if (a is JsonArray aa && b is JsonArray ab)
            {
                if (aa.Count != ab.Count)
                {
                    where = path + ": length " + aa.Count + " != " + ab.Count;
                    return false;
                }
                for (var i = 0; i < aa.Count; i++)
                {
                    if (!Equal(aa[i], ab[i], out where, path + "[" + i + "]")) return false;
                }
                return true;
            }
            if (a is JsonValue va2 && b is JsonValue vb2)
            {
                var ka = va2.GetValueKind();
                var kb = vb2.GetValueKind();
                if (ka == JsonValueKind.Number && kb == JsonValueKind.Number)
                {
                    if (Decimal(va2) == Decimal(vb2)) return true;
                }
                else if (ka == kb && va2.ToJsonString() == vb2.ToJsonString())
                {
                    return true;
                }
            }
            where = path + ": " + Show(a) + " != " + Show(b);
            return false;
        }

        private static decimal Decimal(JsonValue v) => decimal.Parse(v.ToJsonString(), NumberStyles.Float, CultureInfo.InvariantCulture);

        private static bool IsEmptyContainer(JsonNode n) => (n is JsonObject o && o.Count == 0) || (n is JsonArray a && a.Count == 0);

        private static string Show(JsonNode n) => n == null ? "null" : n.ToJsonString();

        /// <summary>Round-trip through JSON text (what "after a JSON round trip" means in the fixture rules).</summary>
        public static JsonNode RoundTrip(JsonNode n) => n == null ? null : JsonNode.Parse(n.ToJsonString());

        /// <summary>PHP parse_str() of a URL's query string, values as strings.</summary>
        public static JsonObject ParseQuery(string url)
        {
            var result = new JsonObject();
            var q = url.IndexOf('?');
            if (q < 0) return result;
            foreach (var pair in url.Substring(q + 1).Split('&'))
            {
                if (pair.Length == 0) continue;
                var eq = pair.IndexOf('=');
                var key = Decode(eq < 0 ? pair : pair.Substring(0, eq));
                var value = eq < 0 ? "" : Decode(pair.Substring(eq + 1));
                var bracket = key.IndexOf('[');
                if (bracket <= 0)
                {
                    result[key] = value;
                    continue;
                }
                var root = key.Substring(0, bracket);
                var subs = new List<string>();
                var rest = key.Substring(bracket);
                while (rest.StartsWith("[", StringComparison.Ordinal))
                {
                    var close = rest.IndexOf(']');
                    if (close < 0) break;
                    subs.Add(rest.Substring(1, close - 1));
                    rest = rest.Substring(close + 1);
                }
                if (!(result[root] is JsonObject cur))
                {
                    cur = new JsonObject();
                    result[root] = cur;
                }
                for (var i = 0; i < subs.Count; i++)
                {
                    var k = subs[i] == "" ? cur.Count.ToString(CultureInfo.InvariantCulture) : subs[i];
                    if (i == subs.Count - 1)
                    {
                        cur[k] = value;
                    }
                    else
                    {
                        if (!(cur[k] is JsonObject next))
                        {
                            next = new JsonObject();
                            cur[k] = next;
                        }
                        cur = next;
                    }
                }
            }
            return result;
        }

        private static string Decode(string s) => WebUtility.UrlDecode(s);

        /// <summary>JsonNode → plain CLR objects (Dictionary, List, string, long, double, bool, null).</summary>
        public static object ToClr(JsonNode n)
        {
            switch (n)
            {
                case null: return null;
                case JsonObject o: return o.ToDictionary(kv => kv.Key, kv => ToClr(kv.Value));
                case JsonArray a: return a.Select(ToClr).ToList();
                case JsonValue v:
                    switch (v.GetValueKind())
                    {
                        case JsonValueKind.String: return v.GetValue<string>();
                        case JsonValueKind.True: return true;
                        case JsonValueKind.False: return false;
                        case JsonValueKind.Number:
                            var raw = v.ToJsonString();
                            if (raw.IndexOfAny(new[] { '.', 'e', 'E' }) < 0 && long.TryParse(raw, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var l)) return l;
                            return double.Parse(raw, NumberStyles.Float, CultureInfo.InvariantCulture);
                        default: return null;
                    }
                default: return null;
            }
        }
    }
}
