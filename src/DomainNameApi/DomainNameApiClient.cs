using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using DomainNameApi.Internal;

namespace DomainNameApi
{
    /// <summary>
    /// Client for the DomainNameAPI reseller REST gateway. A faithful port of
    /// php-dna: same methods, same requests on the wire, same return values.
    ///
    /// Every network method returns the php-dna result structure as a
    /// <see cref="JsonNode"/> (usually a <see cref="JsonObject"/> with
    /// <c>result</c> = <c>OK</c>/<c>ERROR</c>). Failures are returned as an error
    /// envelope, never thrown.
    ///
    /// An instance keeps the last request/response for diagnostics and is not
    /// thread-safe; use one instance per thread or synchronise access.
    /// </summary>
    public sealed partial class DomainNameApiClient : IDisposable
    {
        /// <summary>Library version.</summary>
        public const string Version = "1.0.1";

        /// <summary>User-Agent sent with every gateway request (Cloudflare rejects default client agents).</summary>
        public const string UserAgent = "DomainNameAPI-DotNet/" + Version;

        /// <summary>Application name used to tag telemetry events.</summary>
        public const string ApplicationName = "DomainNameAPI-DotNet";

        /// <summary>Live gateway.</summary>
        public const string UrlProd = "https://api.domainresellerapi.com/api/v1";

        /// <summary>Test (OTE) gateway.</summary>
        public const string UrlOte = "https://ote.domainresellerapi.com/api/v1";

        public const string ResultOk = "OK";
        public const string ResultError = "ERROR";
        public const string ResultSuccess = "SUCCESS";

        public const int DefaultTimeoutSeconds = 30;
        public const int DefaultConnectTimeoutSeconds = 10;
        public const string DefaultReason = "Owner request";

        /// <summary>Nameservers used by RegisterWithContactInfo when none (or only empty ones) are given.</summary>
        public static readonly IReadOnlyList<string> DefaultNameServers = new[] { "ns1.domainnameapi.com", "ns2.domainnameapi.com" };

        /// <summary>Zone record types the gateway accepts.</summary>
        public static readonly IReadOnlyList<string> ZoneRecordTypes = new[]
        {
            "A", "AAAA", "ALIAS", "CAA", "CNAME", "DS", "MX", "NAPTR", "PTR", "SRV", "TLSA", "TXT",
        };

        /// <summary>Redirect types accepted by the forwarding endpoints.</summary>
        public static readonly IReadOnlyList<string> ForwardTypes = new[] { "Standard", "Frame" };

        private static readonly Dictionary<long, string> DomainStatusMap = new Dictionary<long, string>
        {
            [0] = "Active",
            [1] = "WaitingForRegistration",
            [2] = "WaitingForDocument",
            [3] = "WaitingForIncomingTransfer",
            [4] = "TransferredOut",
            [7] = "PendingDelete",
            [8] = "Deleted",
            [9] = "ConfirmationEmailSend",
            [11] = "WaitingForOutgoingTransfer",
            [12] = "PendingHold",
            [15] = "ModificationPending",
            [18] = "InCase",
            [19] = "PendingRestore",
        };

        private readonly string _resellerId;
        private readonly string _apiKey;
        private readonly bool _ownsTransport;
        private string _serviceUrl = UrlProd;
        private bool _sslVerify = true;

        private JsonNode _lastRequest = new JsonArray();
        private JsonNode _lastResponse = new JsonArray();
        private JsonNode _lastResponseHeaders = new JsonArray();
        private JsonNode _lastParsedResponse = new JsonArray();
        private string _lastFunction = "";
        private DateTime _startAt = DateTime.UtcNow;
        private DateTime _lastZoneRequestAt = DateTime.MinValue;
        private bool _transportFailed;
        private string _lastGatewayCode = "";

        /// <summary>
        /// Create a client.
        /// </summary>
        /// <param name="resellerId">Reseller ID (UUID) from the panel's API settings.</param>
        /// <param name="apiKey">API key from the panel's API settings (panel login does not work).</param>
        /// <param name="testMode">Use the OTE test gateway.</param>
        public DomainNameApiClient(string resellerId, string apiKey, bool testMode = false)
            : this(resellerId, apiKey, testMode, null)
        {
        }

        /// <summary>
        /// Create a client with a test-mode flag given as text, as panels store it:
        /// "true", "1", "on" and "yes" (any case) enable test mode, anything else does not.
        /// </summary>
        public DomainNameApiClient(string resellerId, string apiKey, string testMode)
            : this(resellerId, apiKey, ParseBool(testMode), null)
        {
        }

        /// <summary>Create a client with a custom HTTP transport (tests, proxies, instrumentation).</summary>
        public DomainNameApiClient(string resellerId, string apiKey, bool testMode, IHttpTransport transport)
        {
            _resellerId = Php.Trim(resellerId ?? "");
            _apiKey = Php.Trim(apiKey ?? "");
            if (testMode)
            {
                _serviceUrl = UrlOte;
            }
            if (transport == null)
            {
                transport = new HttpClientTransport();
                _ownsTransport = true;
            }
            Transport = transport;
        }

        /// <summary>The HTTP transport. Replace it to intercept requests.</summary>
        public IHttpTransport Transport { get; set; }

        /// <summary>
        /// Minimum gap between two DNS zone calls (the gateway answers HTTP 429 to
        /// bursts). One second by default; tests may set it to zero.
        /// </summary>
        public TimeSpan ZoneMinInterval { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>Total request timeout (30 s).</summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(DefaultTimeoutSeconds);

        /// <summary>Connect timeout (10 s).</summary>
        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(DefaultConnectTimeoutSeconds);

        /// <summary>PHP FILTER_VALIDATE_BOOLEAN for the string test-mode flag (BUG-10284).</summary>
        internal static bool ParseBool(string value)
        {
            var v = Php.Lower(Php.Trim(value ?? ""));
            return v == "1" || v == "true" || v == "on" || v == "yes";
        }

        // ------------------------------------------------------------------
        // Settings and diagnostics
        // ------------------------------------------------------------------

        /// <summary>Turn TLS certificate verification on or off (on by default).</summary>
        public void SetSslVerification(bool enabled) => _sslVerify = enabled;

        /// <summary>The last request: <c>{url, method, payload}</c>.</summary>
        public JsonNode GetRequestData() => Php.Clone(_lastRequest);

        /// <summary>The last decoded response, or <c>{raw_response}</c> when it was not JSON.</summary>
        public JsonNode GetResponseData() => Php.Clone(_lastResponse);

        /// <summary>Transfer info of the last call (url, http_code, content_type, total_time, headers).</summary>
        public JsonNode GetResponseHeaders() => Php.Clone(_lastResponseHeaders);

        /// <summary>The last parsed response, or the error structure built from it.</summary>
        public JsonNode GetParsedResponseData() => Php.Clone(_lastParsedResponse);

        /// <summary>Operation name of the last call, e.g. <c>request:GET domains/info</c>.</summary>
        public string GetLastFunction() => _lastFunction;

        public string GetServiceUrl() => _serviceUrl;

        public void SetServiceUrl(string url) => _serviceUrl = url ?? "";

        public DomainNameApiClient SetRequestData(JsonNode request) { _lastRequest = Php.Clone(request); return this; }
        public DomainNameApiClient SetResponseData(JsonNode response) { _lastResponse = Php.Clone(response); return this; }
        public DomainNameApiClient SetResponseHeaders(JsonNode headers) { _lastResponseHeaders = Php.Clone(headers); return this; }
        public DomainNameApiClient SetParsedResponseData(JsonNode response) { _lastParsedResponse = Php.Clone(response); return this; }
        public DomainNameApiClient SetLastFunction(string function) { _lastFunction = function ?? ""; return this; }

        public void Dispose()
        {
            if (_ownsTransport && Transport is IDisposable d)
            {
                d.Dispose();
            }
        }

        // ------------------------------------------------------------------
        // Errors
        // ------------------------------------------------------------------

        /// <summary>An API/transport failure inside a public method (php-dna throws \Exception).</summary>
        internal sealed class ApiException : Exception
        {
            public ApiException(string message, long code) : base(message)
            {
                Code = code;
            }

            public long Code { get; }
        }

        private static readonly Dictionary<string, (string Message, string Description)> DefaultErrors =
            new Dictionary<string, (string, string)>
            {
                ["DOMAIN_DETAILS"] = ("Invalid domain details! Details format is not valid", "The provided domain details are not in the expected format"),
                ["CREDENTIALS"] = ("Invalid reseller ID or API key", "The provided API credentials are invalid"),
                ["DOMAIN_LIST"] = ("The domain list is invalid or contains multiple entries!", "The domain list response format is incorrect or unexpected"),
                ["TLD_LIST"] = ("TLD info is not a valid array or more than one TLD info has returned!", "The TLD list response is not in the expected format"),
                ["RESPONSE"] = ("Invalid response received from server! Response is empty.", "The API response is empty or null"),
                ["RESPONSE_FORMAT"] = ("Invalid response received from server! Response format is not valid.", "The API response format is not in the expected structure"),
                ["RESPONSE_COUNT"] = ("Invalid parameters passed to function! Response data contains more than one result!", "The API response contains multiple results when only one was expected"),
                ["RESPONSE_CODE"] = ("Invalid parameters passed to function! Operation result or Error code not received from server", "The API response is missing required operation result or error code fields"),
                ["RESPONSE_REST"] = ("Invalid parameters passed to function! REST return is not a valid array!", "The REST response is not in a valid array format"),
                ["CONTACT_INFO"] = ("Invalid response received from server! Contact info is not a valid array or more than one contact info has returned!", "The contact information response is not in the expected format"),
                ["CONTACT_SAVE"] = ("Invalid response received from server! Contact info could not be saved!", "The contact information could not be saved on the server"),
                ["DOMAIN_TRANSFER_REQUEST"] = ("Invalid response received from server! Domain transfer request could not be completed!", "The domain transfer request failed to complete"),
                ["DOMAIN_RENEW"] = ("Invalid response received from server! Domain renew request could not be completed!", "The domain renewal request failed to complete"),
                ["DOMAIN_REGISTER"] = ("Invalid response received from server! Domain register request could not be completed!", "The domain registration request failed to complete"),
                ["DOMAIN_SYNC"] = ("Invalid response received from server! Domain sync request could not be completed!", "The domain synchronization request failed to complete"),
            };

        /// <summary>php-dna setError(): {Code, Message, Details}.</summary>
        private JsonObject SetError(string code, JsonNode message = null, JsonNode details = null)
        {
            message = message ?? Php.Str("");
            details = details ?? Php.Str("");
            JsonObject result;
            if (DefaultErrors.TryGetValue(code, out var def))
            {
                result = new JsonObject
                {
                    ["Code"] = code,
                    ["Message"] = Php.Trim(Php.ToStr(message)) != "" ? Php.Clone(message) : Php.Str(def.Message),
                    ["Details"] = Php.Trim(Php.ToStr(details)) != "" ? Php.Clone(details) : Php.Str(def.Description),
                };
            }
            else
            {
                result = new JsonObject
                {
                    ["Code"] = code,
                    ["Message"] = Php.Clone(message),
                    ["Details"] = Php.Clone(details),
                };
            }
            // Keep the gateway's own error code next to the API_<status> code.
            if (ApiStatusCode.IsMatch(code ?? "") && _lastGatewayCode != "")
            {
                result["GatewayCode"] = _lastGatewayCode;
            }
            return result;
        }

        private JsonObject SetError(string code, string message, string details) => SetError(code, Php.Str(message), Php.Str(details));

        private static readonly Regex ApiStatusCode = new Regex(@"^API_[0-9]+$", RegexOptions.CultureInvariant);

        /// <summary>formatErrorCode(): HTTP statuses become API_&lt;status&gt;, transport failures CURL_&lt;errno&gt;.</summary>
        private string FormatErrorCode(long code) => (_transportFailed ? "CURL_" : "API_") + code.ToString(CultureInfo.InvariantCulture);

        /// <summary>The catch block shared by every public method.</summary>
        private JsonObject ErrorEnvelope(Exception e, string fallbackCode)
        {
            var code = e is ApiException api ? FormatErrorCode(api.Code) : fallbackCode;
            return new JsonObject
            {
                ["result"] = ResultError,
                ["error"] = SetError(code, Php.Str(e.Message), Php.Clone(CatchDetails(e))),
            };
        }

        /// <summary>$this->lastParsedResponse['Details'] ?? ($this->lastResponse['raw_response'] ?? $e->getMessage()).</summary>
        private JsonNode CatchDetails(Exception e)
        {
            return Php.Coalesce(Php.Get(_lastParsedResponse, "Details"), Php.Get(_lastResponse, "raw_response"), Php.Str(e.Message));
        }

        private static JsonObject Ok(JsonNode data) => new JsonObject { ["result"] = ResultOk, ["data"] = data };

        // ------------------------------------------------------------------
        // HTTP
        // ------------------------------------------------------------------

        private JsonNode Get(string endpoint, JsonNode data = null) => Request("GET", endpoint, data);
        private JsonNode Post(string endpoint, JsonNode data = null) => Request("POST", endpoint, data);
        private JsonNode Put(string endpoint, JsonNode data = null) => Request("PUT", endpoint, data);
        private JsonNode Delete(string endpoint, JsonNode data = null) => Request("DELETE", endpoint, data);

        private static readonly Dictionary<int, string> HttpReasons = new Dictionary<int, string>
        {
            [400] = "Bad Request",
            [401] = "Unauthorized",
            [402] = "Payment Required",
            [403] = "Forbidden",
            [404] = "Not Found",
            [405] = "Method Not Allowed",
            [408] = "Request Timeout",
            [409] = "Conflict",
            [422] = "Unprocessable Entity",
            [429] = "Too Many Requests",
            [500] = "Internal Server Error",
            [502] = "Bad Gateway",
            [503] = "Service Unavailable",
            [504] = "Gateway Timeout",
        };

        private static string HttpStatusReason(int status)
        {
            if (HttpReasons.TryGetValue(status, out var r)) return r;
            if (status >= 500) return "Server Error";
            if (status >= 400) return "Client Error";
            return "Unexpected Response";
        }

        private static readonly Regex Boilerplate = new Regex("Exception of type '[^']*' was thrown", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /// <summary>
        /// Port of DNARest::request(). Returns the decoded body of a 2xx response
        /// (null when empty or not JSON) and throws <see cref="ApiException"/> for
        /// HTTP errors and transport failures.
        /// </summary>
        private JsonNode Request(string method, string endpoint, JsonNode data = null)
        {
            data = data ?? new JsonArray();
            PaceZoneRequest(endpoint);

            _transportFailed = false;
            _lastGatewayCode = "";

            _startAt = DateTime.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            var opPath = (endpoint ?? "").Split(new[] { '?' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
            _lastFunction = "request:" + method + " " + opPath;

            var url = _serviceUrl + "/" + Php.LTrim(endpoint, "/");
            _lastRequest = new JsonObject { ["url"] = url, ["method"] = method, ["payload"] = Php.Clone(data) };

            string body = null;
            if (method == "GET" || method == "DELETE")
            {
                if (!Php.Empty(data))
                {
                    url += "?" + Php.BuildQuery(data);
                }
            }
            else
            {
                body = Php.JsonEncode(data);
            }

            var request = new HttpRequestData
            {
                Method = method,
                Url = url,
                Body = body,
                Timeout = Timeout,
                ConnectTimeout = ConnectTimeout,
                VerifySsl = _sslVerify,
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Accept"] = "application/json",
                    ["X-API-KEY"] = _apiKey,
                    ["__reseller"] = _resellerId,
                    ["User-Agent"] = UserAgent,
                },
            };

            HttpResponseData response;
            try
            {
                response = Transport.Send(request);
                if (response == null)
                {
                    throw new TransportException(52, "Empty reply from server");
                }
            }
            catch (TransportException te)
            {
                // Transport failure: never an empty success.
                _lastResponseHeaders = new JsonObject { ["url"] = url, ["http_code"] = 0, ["content_type"] = null, ["total_time"] = stopwatch.Elapsed.TotalSeconds };
                _lastResponse = new JsonObject { ["raw_response"] = false };
                _transportFailed = true;
                var errorMessage = "Curl error during request: " + te.Message;
                if (te.ErrorNumber == 28)
                {
                    errorMessage = "Could not reach the DomainNameAPI service: the connection timed out. (" + te.Message + ")";
                }
                _lastParsedResponse = SetError("CURL_" + te.ErrorNumber.ToString(CultureInfo.InvariantCulture), errorMessage, errorMessage);
                var error = new ApiException(errorMessage, te.ErrorNumber);
                SendErrorToSentryAsync(error);
                throw error;
            }

            var status = response.StatusCode;
            var responseBody = response.Body ?? "";
            var headersNode = new JsonObject();
            foreach (var h in response.Headers ?? new Dictionary<string, string>()) headersNode[h.Key] = h.Value;
            _lastResponseHeaders = new JsonObject
            {
                ["url"] = url,
                ["content_type"] = response.ContentType ?? "",
                ["http_code"] = status,
                ["total_time"] = response.Elapsed.TotalSeconds,
                ["headers"] = headersNode,
            };
            var decoded = Php.Decode(responseBody);
            _lastResponse = decoded ?? new JsonObject { ["raw_response"] = responseBody };

            JsonNode parsed;
            if (status >= 200 && status <= 299)
            {
                parsed = decoded;
                _lastParsedResponse = Php.Clone(parsed);

                // Successful calls are sampled for performance metrics; slow calls always.
                var duration = stopwatch.Elapsed.TotalMilliseconds;
                if (duration > 1000 || ThreadSafeRandom.Next(1, 1001) <= PerformanceSampleRate)
                {
                    SendPerformanceMetricsToSentry(_lastFunction, duration, _startAt, DateTime.UtcNow);
                }
                return parsed;
            }

            parsed = decoded;
            if (status == 302 || status == 301)
            {
                parsed = new JsonObject
                {
                    ["message"] = "Invalid API credentials",
                    ["code"] = "CREDENTIALS",
                    ["details"] = "Authentication failed. Check your API key and reseller ID.",
                };
            }

            var messageNode = Php.Coalesce(Php.Get(parsed, "message"), Php.Get(parsed, "error", "message"), Php.Str(responseBody));
            var codeNode = Php.Coalesce(Php.Get(parsed, "code"), Php.Get(parsed, "error", "code"), Php.Str("HTTP_" + status.ToString(CultureInfo.InvariantCulture)));
            var apiDetails = Php.Coalesce(Php.Get(parsed, "details"), Php.Get(parsed, "error", "details"));
            var detailsNode = Php.Coalesce(apiDetails, Php.Str(responseBody));
            if (Php.IsString(codeNode))
            {
                var gw = codeNode.GetValue<string>();
                if (gw != "" && !gw.StartsWith("HTTP_", StringComparison.Ordinal) && gw != "CREDENTIALS")
                {
                    _lastGatewayCode = gw;
                }
            }

            var message = Php.ToStr(messageNode);
            var codeText = Php.ToStr(codeNode);
            if (Php.Trim(message) == "")
            {
                message = "HTTP " + status.ToString(CultureInfo.InvariantCulture) + " - " + HttpStatusReason(status);
                if (Php.Trim(Php.ToStr(detailsNode)) == "")
                {
                    detailsNode = Php.Str(message);
                }
            }

            // ABP boilerplate or the bare error code echoed back (BUG-10124).
            if (Boilerplate.IsMatch(message) || message == codeText)
            {
                // Only the gateway's details field may replace the message, never the raw body.
                var detailsClean = Php.Trim(Regex.Replace(Php.ToStr(apiDetails), @"^Dna[ \t\n\r\v\f]+", ""));
                if (detailsClean != "" && detailsClean != message && detailsClean != codeText && !Boilerplate.IsMatch(detailsClean))
                {
                    message = detailsClean;
                }
                else
                {
                    message = "The operation could not be completed by the registry (error: " + codeText + "). Please try again or contact support.";
                }
            }

            var validationErrors = Php.Coalesce(Php.Get(parsed, "error", "validationErrors"), Php.Get(parsed, "validationErrors"));
            if (Php.IsArray(validationErrors) && !Php.Empty(validationErrors))
            {
                var flattened = FlattenValidationErrors(validationErrors);
                if (flattened != "")
                {
                    detailsNode = Php.Str(flattened);
                }
            }

            _lastParsedResponse = SetError(codeText, Php.Str(message), Php.Clone(detailsNode));
            var apiError = new ApiException(message, status);
            SendErrorToSentryAsync(apiError);
            throw apiError;
        }

        /// <summary>Keep consecutive /domains/zones calls at least <see cref="ZoneMinInterval"/> apart.</summary>
        private void PaceZoneRequest(string endpoint)
        {
            if (!Php.LTrim(endpoint ?? "", "/").StartsWith("domains/zones", StringComparison.Ordinal))
            {
                return;
            }
            if (_lastZoneRequestAt != DateTime.MinValue && ZoneMinInterval > TimeSpan.Zero)
            {
                var wait = ZoneMinInterval - (DateTime.UtcNow - _lastZoneRequestAt);
                if (wait > TimeSpan.Zero)
                {
                    Thread.Sleep(wait);
                }
            }
            _lastZoneRequestAt = DateTime.UtcNow;
        }

        /// <summary>Flatten error.validationErrors[] into "Field, Field - message | ...".</summary>
        internal static string FlattenValidationErrors(JsonNode validationErrors)
        {
            var parts = new List<string>();
            foreach (var ve in Php.Values(validationErrors))
            {
                if (Php.IsString(ve))
                {
                    var s = Php.Trim(ve.GetValue<string>());
                    if (s != "") parts.Add(s);
                    continue;
                }
                if (!Php.IsArray(ve))
                {
                    continue;
                }
                var messageNode = Php.Get(ve, "message");
                var message = messageNode != null ? Php.Trim(Php.ToStr(messageNode)) : "";
                var members = new List<string>();
                var membersNode = Php.Get(ve, "members");
                if (Php.IsArray(membersNode))
                {
                    foreach (var member in Php.Values(membersNode))
                    {
                        var label = HumanizeValidationMember(Php.ToStr(member));
                        if (label != "") members.Add(label);
                    }
                }
                if (members.Count > 0 && message != "") parts.Add(string.Join(", ", members) + " - " + message);
                else if (members.Count > 0) parts.Add(string.Join(", ", members));
                else if (message != "") parts.Add(message);
            }
            return string.Join(" | ", parts.Distinct());
        }

        private static readonly Dictionary<string, string> ValidationRoles = new Dictionary<string, string>
        {
            ["administrativecontact"] = "Admin",
            ["administrative"] = "Admin",
            ["technicalcontact"] = "Tech",
            ["technical"] = "Tech",
            ["registrantcontact"] = "Registrant",
            ["registrant"] = "Registrant",
            ["billingcontact"] = "Billing",
            ["billing"] = "Billing",
        };

        /// <summary>"$.administrativeContact.postalCode" → "Admin PostalCode".</summary>
        internal static string HumanizeValidationMember(string member)
        {
            member = Php.Trim(member ?? "");
            if (member == "") return "";
            member = Php.LTrim(member, "$.");
            member = Regex.Replace(member, @"\[[0-9]+\]", "");
            var segments = Regex.Split(member, @"[./]+").Where(s => s.Length > 0).ToList();
            if (segments.Count == 0) return "";
            var output = segments.Select(seg => ValidationRoles.TryGetValue(Php.Lower(seg), out var role) ? role : Php.UcFirst(seg));
            return Php.Trim(string.Join(" ", output));
        }

        private static class ThreadSafeRandom
        {
            private static readonly Random Global = new Random();
            [ThreadStatic] private static Random _local;

            public static int Next(int min, int max)
            {
                if (_local == null)
                {
                    int seed;
                    lock (Global) seed = Global.Next();
                    _local = new Random(seed);
                }
                return _local.Next(min, max);
            }
        }
    }
}
