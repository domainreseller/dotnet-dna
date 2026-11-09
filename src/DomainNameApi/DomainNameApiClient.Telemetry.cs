using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DomainNameApi.Internal;

namespace DomainNameApi
{
    public sealed partial class DomainNameApiClient
    {
        /// <summary>
        /// Sentry DSN for the .NET library. Empty until the Sentry project exists;
        /// it is filled in then. With an empty DSN telemetry is a no-op.
        /// </summary>
        public const string DefaultSentryDsn = "";

        /// <summary>Successful calls sampled for performance metrics, per 1000 (4%).</summary>
        public const int PerformanceSampleRate = 40;

        /// <summary>Error messages that are expected noise and never reported.</summary>
        public static readonly IReadOnlyList<string> DefaultIgnoredErrors = new[]
        {
            "Domain not found",
            "ERR_DOMAIN_NOT_FOUND",
            "Reseller not found",
            "Domain is not in updateable status",
            "balance is not sufficient",
            "Price definition not found",
            "TLD is not supported",
            "Invalid API credentials",
            "could not be found",
            "Insufficent reseller balance",
            "Redirect information not found",
        };

        private bool _errorReportingEnabled = true;
        private bool _telemetryMinimal;
        private string _sessionTraceId;

        /// <summary>Sentry DSN used for telemetry. Defaults to <see cref="DefaultSentryDsn"/>.</summary>
        public string TelemetryDsn { get; set; } = DefaultSentryDsn;

        /// <summary>
        /// Delivery of an encoded Sentry event: (store URL, JSON body, X-Sentry-Auth header).
        /// Replaceable for tests; the default posts in the background and never blocks or throws.
        /// </summary>
        internal Action<string, string, string> TelemetrySender { get; set; } = FireAndForgetPost;

        /// <summary>Turn error and performance reporting on or off (on by default).</summary>
        public void SetErrorReportingEnabled(bool enabled) => _errorReportingEnabled = enabled;

        /// <summary>
        /// Minimal telemetry: only the error code and message, endpoint, timing,
        /// versions and reseller ID are sent — no request/response data, domain,
        /// server IP or account name.
        /// </summary>
        public void SetTelemetryMinimal(bool minimal) => _telemetryMinimal = minimal;

        private sealed class Dsn
        {
            public string StoreUrl;
            public string PublicKey;
            public string SecretKey;
        }

        private static Dsn ParseDsn(string dsn)
        {
            if (string.IsNullOrWhiteSpace(dsn)) return null;
            if (!Uri.TryCreate(dsn, UriKind.Absolute, out var uri)) return null;
            if (string.IsNullOrEmpty(uri.UserInfo) || string.IsNullOrEmpty(uri.Host)) return null;
            var projectId = uri.AbsolutePath.TrimStart('/');
            if (projectId == "") return null;
            var userInfo = uri.UserInfo.Split(new[] { ':' }, 2);
            var host = uri.IsDefaultPort ? uri.Host : uri.Host + ":" + uri.Port.ToString(CultureInfo.InvariantCulture);
            return new Dsn
            {
                StoreUrl = "https://" + host + "/api/" + projectId + "/store/",
                PublicKey = Uri.UnescapeDataString(userInfo[0]),
                SecretKey = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
            };
        }

        private static string AuthHeader(Dsn dsn, string client)
        {
            var parts = new List<string> { "sentry_version=7", "sentry_client=" + client + "/" + Version, "sentry_key=" + dsn.PublicKey };
            if (!string.IsNullOrEmpty(dsn.SecretKey)) parts.Add("sentry_secret=" + dsn.SecretKey);
            return "Sentry " + string.Join(", ", parts);
        }

        /// <summary>Report an error. Never throws into the caller.</summary>
        private void SendErrorToSentryAsync(ApiException e)
        {
            try
            {
                var payload = BuildErrorEvent(e);
                if (payload == null) return;
                TelemetrySender?.Invoke(payload.Value.Url, payload.Value.Body, payload.Value.Auth);
            }
            catch
            {
                // Telemetry must never surface in the host application.
            }
        }

        /// <summary>Build the Sentry error event, or null when it must not be sent.</summary>
        internal (string Url, string Body, string Auth)? BuildErrorEvent(ApiException e)
        {
            if (!_errorReportingEnabled) return null;
            foreach (var ignored in DefaultIgnoredErrors)
            {
                if (e.Message.IndexOf(ignored, StringComparison.Ordinal) >= 0) return null;
            }
            var dsn = ParseDsn(TelemetryDsn);
            if (dsn == null) return null;

            var elapsed = (DateTime.UtcNow - _startAt).TotalSeconds;
            var classification = BuildSentryClassification(e);
            var culprit = typeof(DomainNameApiClient).FullName;

            var tags = new JsonObject
            {
                ["handled"] = "yes",
                ["level"] = "error",
                ["url"] = "NA",
                ["transaction"] = "NA",
                ["trace_id"] = RandomHex(8),
                ["runtime_name"] = ".NET",
                ["runtime_version"] = RuntimeInformation.FrameworkDescription,
                ["elapsed_time"] = elapsed.ToString("F4", CultureInfo.InvariantCulture),
                ["application"] = ApplicationName,
                ["library_version"] = Version,
                ["ip_address"] = _telemetryMinimal ? "" : ServerIp.Get(),
                ["vhost_user"] = SafeUserName(),
            };
            foreach (var kv in classification.Tags) tags[kv.Key] = kv.Value;

            var frames = Frames();
            var errorData = new JsonObject
            {
                ["event_id"] = RandomHex(16),
                ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture),
                ["level"] = "error",
                ["logger"] = "csharp",
                ["platform"] = "csharp",
                ["culprit"] = culprit,
                ["message"] = new JsonObject { ["formatted"] = classification.Value },
                ["exception"] = new JsonObject
                {
                    ["values"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["type"] = ApplicationName + " Exception",
                            ["value"] = classification.Value,
                            ["stacktrace"] = new JsonObject { ["frames"] = frames },
                        },
                    },
                },
                ["tags"] = tags,
                ["extra"] = new JsonObject
                {
                    ["request_data"] = TelemetryMask(_lastRequest),
                    ["response_data"] = TelemetryMask(_lastResponse),
                    ["original_message"] = e.Message,
                },
                ["release"] = Version,
                ["environment"] = GetEnvironment(),
                ["fingerprint"] = new JsonArray(classification.Fingerprint.Select(Php.Str).ToArray()),
            };

            if (_telemetryMinimal)
            {
                tags.Remove("url");
                tags.Remove("ip_address");
                tags.Remove("vhost_user");
                tags.Remove("ip_value");
                var extra = (JsonObject)errorData["extra"];
                string endpoint = "";
                if (Uri.TryCreate(Php.ToStr(Php.Get(_lastRequest, "url")), UriKind.Absolute, out var u)) endpoint = u.AbsolutePath;
                extra["request_data"] = new JsonObject
                {
                    ["method"] = Php.ToStr(Php.Get(_lastRequest, "method")),
                    ["endpoint"] = endpoint,
                };
                extra["response_data"] = new JsonObject
                {
                    ["http_code"] = Php.Int(Php.ToInt(Php.Get(_lastResponseHeaders, "http_code"))),
                };
            }

            return (dsn.StoreUrl, errorData.ToJsonString(), AuthHeader(dsn, "dotnet-dna"));
        }

        /// <summary>Report a sampled successful call. Never throws.</summary>
        private void SendPerformanceMetricsToSentry(string operation, double durationMs, DateTime start, DateTime end)
        {
            try
            {
                var payload = BuildPerformanceEvent(operation, durationMs, start, end);
                if (payload == null) return;
                TelemetrySender?.Invoke(payload.Value.Url, payload.Value.Body, payload.Value.Auth);
            }
            catch
            {
                // Fail silently.
            }
        }

        internal (string Url, string Body, string Auth)? BuildPerformanceEvent(string operation, double durationMs, DateTime start, DateTime end)
        {
            if (!_errorReportingEnabled) return null;
            var dsn = ParseDsn(TelemetryDsn);
            if (dsn == null) return null;

            var traceId = _sessionTraceId ?? (_sessionTraceId = RandomHex(16));
            var rootSpan = RandomHex(8);
            var childSpan = RandomHex(8);

            var tags = new JsonObject
            {
                ["application"] = ApplicationName,
                ["library_version"] = Version,
                ["ip_address"] = _telemetryMinimal ? "" : ServerIp.Get(),
                ["vhost_user"] = _telemetryMinimal ? "" : SafeUserName(),
            };
            foreach (var kv in BuildCommonTags(null)) tags[kv.Key] = kv.Value;

            var httpCode = Php.ToInt(Php.Get(_lastResponseHeaders, "http_code"));
            if (httpCode != 0) tags["status_code"] = httpCode.ToString(CultureInfo.InvariantCulture);

            var method = Php.ToStr(Php.Coalesce(Php.Get(_lastRequest, "method"), Php.Str("GET")));
            var callUrl = Php.ToStr(Php.Get(_lastRequest, "url"));
            var callHost = Uri.TryCreate(callUrl != "" ? callUrl : _serviceUrl, UriKind.Absolute, out var u) ? u.Host : "";

            var spanData = new JsonObject { ["http.request.method"] = method };
            if (callHost != "") spanData["server.address"] = callHost;
            if (callUrl != "" && !_telemetryMinimal) spanData["url.full"] = callUrl.Split('?')[0];
            if (httpCode != 0) spanData["http.response.status_code"] = Php.Int(httpCode);

            var description = callHost != "" ? method + " https://" + callHost : operation;
            var startTs = Iso(start);
            var endTs = Iso(end);

            var evt = new JsonObject
            {
                ["event_id"] = RandomHex(16),
                ["platform"] = "csharp",
                ["level"] = "info",
                ["type"] = "transaction",
                ["transaction"] = "API." + operation,
                ["transaction_info"] = new JsonObject { ["source"] = "custom" },
                ["release"] = Version,
                ["environment"] = GetEnvironment(),
                ["timestamp"] = endTs,
                ["start_timestamp"] = startTs,
                ["contexts"] = new JsonObject
                {
                    ["trace"] = new JsonObject { ["trace_id"] = traceId, ["span_id"] = rootSpan, ["op"] = "http.client", ["status"] = "ok" },
                    ["runtime"] = new JsonObject { ["name"] = ".NET", ["version"] = RuntimeInformation.FrameworkDescription },
                },
                ["tags"] = tags,
                ["measurements"] = new JsonObject
                {
                    ["duration"] = new JsonObject { ["value"] = durationMs, ["unit"] = "millisecond" },
                    ["memory"] = new JsonObject { ["value"] = Php.Int(GC.GetTotalMemory(false)), ["unit"] = "byte" },
                },
                ["spans"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["span_id"] = childSpan,
                        ["parent_span_id"] = rootSpan,
                        ["trace_id"] = traceId,
                        ["op"] = "http.client",
                        ["description"] = description,
                        ["status"] = "ok",
                        ["data"] = spanData,
                        ["start_timestamp"] = startTs,
                        ["timestamp"] = endTs,
                    },
                },
            };
            return (dsn.StoreUrl, evt.ToJsonString(), AuthHeader(dsn, "dotnet-dna"));
        }

        private static string Iso(DateTime t) => t.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);

        private string GetEnvironment() => (_serviceUrl ?? "").IndexOf("ote.", StringComparison.OrdinalIgnoreCase) >= 0 ? "ote" : "production";

        private static readonly Regex MaskKeys = new Regex("auth|epp|password|passwd|secret|token|api[_-]?key", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /// <summary>Replace credential values (auth/EPP codes, API keys, passwords, tokens) with ***.</summary>
        internal static JsonNode TelemetryMask(JsonNode data, int depth = 0)
        {
            if (depth > 8) return Php.Str("[…]");
            if (!Php.IsArray(data)) return Php.Clone(data);
            var output = new JsonObject();
            var isList = data is JsonArray;
            var list = new JsonArray();
            foreach (var kv in Php.Entries(data))
            {
                JsonNode masked;
                if (MaskKeys.IsMatch(kv.Key))
                {
                    masked = kv.Value == null || IsEmptyString(kv.Value) ? Php.Clone(kv.Value) : Php.Str("***");
                }
                else
                {
                    masked = TelemetryMask(kv.Value, depth + 1);
                }
                if (isList) list.Add(masked);
                else output[kv.Key] = masked;
            }
            return isList ? (JsonNode)list : output;
        }

        /// <summary>Tags shared by error and transaction events.</summary>
        private Dictionary<string, string> BuildCommonTags(ApiException e)
        {
            var tags = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(_resellerId)) tags["reseller"] = _resellerId;

            var domain = Php.Get(_lastRequest, "payload", "domainName");
            if (Php.IsString(domain) && !Php.Empty(domain) && !_telemetryMinimal) tags["domain"] = domain.GetValue<string>();

            if (!string.IsNullOrEmpty(_lastFunction)) tags["operation"] = _lastFunction;
            tags["api_type"] = "REST";

            if (e != null)
            {
                var m = Regex.Match(e.Message, @"API_(-?\d+)_ERROR");
                if (m.Success) tags["api_code"] = "API_" + m.Groups[1].Value;
                else if (e.Code != 0) tags["api_code"] = "API_" + e.Code.ToString(CultureInfo.InvariantCulture);
                if (e.Code != 0) tags["status_code"] = e.Code.ToString(CultureInfo.InvariantCulture);
            }
            return tags;
        }

        internal sealed class Classification
        {
            public string Value;
            public Dictionary<string, string> Tags;
            public string[] Fingerprint;
        }

        /// <summary>Stable title and fingerprint: volatile parts of the message become placeholders.</summary>
        internal Classification BuildSentryClassification(ApiException e)
        {
            var msg = e.Message ?? "";
            var tags = BuildCommonTags(e);

            var ip = Regex.Match(msg, @"\b(\d{1,3}(?:\.\d{1,3}){3})\b");
            if (ip.Success) tags["ip_value"] = ip.Groups[1].Value;

            tags.TryGetValue("api_code", out var apiCode);
            var n = msg;
            n = Regex.Replace(n, @"^\[API_ERROR\]:\s*", "", RegexOptions.IgnoreCase);
            n = Regex.Replace(n, @"API_-?\d+_ERROR\s*-\s*Failed\s*-\s*", "", RegexOptions.IgnoreCase);
            n = Regex.Replace(n, @"\s*\(Reseller Id[^)]*\)\s*", "", RegexOptions.IgnoreCase);
            n = Regex.Replace(n, @"\b\d{1,3}(?:\.\d{1,3}){3}\b", "{ip}");
            n = Regex.Replace(n, "'[^']{0,80}'|\"[^\"]{0,80}\"", "{v}");
            n = Regex.Replace(n, @"\b\d{2,}\b", "{n}");
            n = Regex.Replace(n, @"\s+", " ").Trim();

            if (n == "")
            {
                n = !string.IsNullOrEmpty(apiCode) ? apiCode : "ApiException";
            }
            else if (!string.IsNullOrEmpty(apiCode) && !n.StartsWith(apiCode, StringComparison.Ordinal))
            {
                n = apiCode + ": " + n;
            }
            if (n.Length > 160) n = n.Substring(0, 157) + "...";

            tags.TryGetValue("operation", out var operation);
            return new Classification
            {
                Value = n,
                Tags = tags,
                Fingerprint = new[] { "dna", ApplicationName, operation ?? "unknown", n },
            };
        }

        private static JsonArray Frames()
        {
            var frames = new JsonArray();
            var trace = new StackTrace(2, true).GetFrames() ?? Array.Empty<StackFrame>();
            foreach (var f in trace.Reverse())
            {
                var method = f.GetMethod();
                var file = f.GetFileName();
                frames.Add(new JsonObject
                {
                    ["filename"] = string.IsNullOrEmpty(file) ? "" : System.IO.Path.GetFileName(file),
                    ["lineno"] = f.GetFileLineNumber(),
                    ["function"] = method == null ? "" : (method.DeclaringType?.FullName + "::" + method.Name),
                });
            }
            return frames;
        }

        private static string RandomHex(int bytes)
        {
            var buf = new byte[bytes];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(buf);
            var sb = new StringBuilder(bytes * 2);
            foreach (var b in buf) sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            return sb.ToString();
        }

        private static string SafeUserName()
        {
            try
            {
                return Environment.UserName ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static readonly HttpClient TelemetryHttp = new HttpClient { Timeout = TimeSpan.FromMilliseconds(500) };

        private static void FireAndForgetPost(string url, string body, string auth)
        {
            Task.Run(async () =>
            {
                try
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                        request.Headers.TryAddWithoutValidation("X-Sentry-Auth", auth);
                        request.Headers.TryAddWithoutValidation("User-Agent", UserAgent);
                        using (await TelemetryHttp.SendAsync(request).ConfigureAwait(false))
                        {
                        }
                    }
                }
                catch
                {
                    // Fire and forget.
                }
            });
        }

        /// <summary>
        /// The server's public IP (api.ipify.org), cached for 512 seconds. Never
        /// blocks: a missing or stale value is refreshed in the background and
        /// "unknown" is reported meanwhile.
        /// </summary>
        private static class ServerIp
        {
            private static readonly object Lock = new object();
            private static string _ip;
            private static DateTime _fetchedAt = DateTime.MinValue;
            private static bool _refreshing;

            public static string Get()
            {
                lock (Lock)
                {
                    var fresh = _ip != null && (DateTime.UtcNow - _fetchedAt).TotalSeconds < 512;
                    if (!fresh && !_refreshing)
                    {
                        _refreshing = true;
                        Task.Run(Refresh);
                    }
                    return _ip ?? "unknown";
                }
            }

            private static async Task Refresh()
            {
                try
                {
                    using (var http = new HttpClient { Timeout = TimeSpan.FromSeconds(2) })
                    {
                        var text = (await http.GetStringAsync("https://api.ipify.org").ConfigureAwait(false)).Trim();
                        if (IPAddress.TryParse(text, out _))
                        {
                            lock (Lock)
                            {
                                _ip = text;
                                _fetchedAt = DateTime.UtcNow;
                            }
                        }
                    }
                }
                catch
                {
                    // stays unknown
                }
                finally
                {
                    lock (Lock) _refreshing = false;
                }
            }
        }
    }
}
