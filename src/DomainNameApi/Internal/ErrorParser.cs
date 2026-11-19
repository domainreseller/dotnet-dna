using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DomainNameApi.Internal;

/// <summary>Turns a failed answer into a <see cref="DomainNameApiException"/> with a stable kind, code and hint.</summary>
internal static class ErrorParser
{
    // The generic .NET exception text the API sometimes sends instead of a useful message.
    private static readonly Regex Boilerplate =
        new("exception of type '[^']*' was thrown", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static DomainNameApiException FromResponse(
        BaseRequest request,
        int status,
        IReadOnlyDictionary<string, string> headers,
        string body,
        bool redirectedToErrorPage = false)
    {
        headers.TryGetValue("X-Correlation-Id", out var correlationId);
        correlationId = string.IsNullOrEmpty(correlationId) ? null : correlationId;

        // The multi-reseller middleware redirects to /Error/Suspend when the __reseller value is
        // unknown, inactive or suspended in this environment.
        if (status is 301 or 302 || redirectedToErrorPage)
        {
            return Create(request, ApiErrorKind.Authentication, "INVALID_RESELLER",
                "Authentication failed: the reseller ID is unknown, inactive or suspended in this environment.",
                status, correlationId, body);
        }

        if (status == 429)
        {
            var retryAfter = RetryAfter(headers);
            var message = "Rate limit exceeded.";
            if (retryAfter is not null)
            {
                message = "Rate limit exceeded; retry after " + retryAfter.Value.TotalSeconds.ToString("0.##", CultureInfo.InvariantCulture) + " s.";
            }
            return Create(request, ApiErrorKind.RateLimited, "RATE_LIMITED", message, status, correlationId, body, retryAfter: retryAfter);
        }

        string code = "", text = "", details = "";
        var validationErrors = new List<ApiValidationError>();
        var trimmed = body.Trim();
        if (trimmed.StartsWith("{", StringComparison.Ordinal) && TryReadJson(trimmed, out var root))
        {
            // ABP wraps errors as {"error": {...}}; some endpoints answer with a flat object.
            var error = root.TryGetProperty("error", out var e) && e.ValueKind == JsonValueKind.Object ? e : root;
            code = ReadString(error, "code");
            text = ReadString(error, "message");
            details = ReadString(error, "details");
            if (error.TryGetProperty("validationErrors", out var list) && list.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in list.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String)
                    {
                        validationErrors.Add(new ApiValidationError { Message = item.GetString() ?? "" });
                        continue;
                    }
                    if (item.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }
                    var members = new List<string>();
                    if (item.TryGetProperty("members", out var m) && m.ValueKind == JsonValueKind.Array)
                    {
                        members.AddRange(m.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString() ?? ""));
                    }
                    validationErrors.Add(new ApiValidationError { Message = ReadString(item, "message"), Members = members });
                }
            }
        }
        else if (trimmed.Length > 0)
        {
            text = trimmed;
        }

        if (status == 401 && code.Length == 0)
        {
            var lower = text.ToLowerInvariant();
            code = lower.Contains("api key is missing") ? "MISSING_API_KEY"
                : lower.Contains("resellerid is missing") ? "MISSING_RESELLER_ID"
                : lower.Contains("invalid api key") ? "INVALID_API_KEY"
                : lower.Contains("authorisation") || lower.Contains("authorization") ? "SUBRESELLER_NOT_ALLOWED"
                : "UNAUTHORIZED";
        }

        // Replace boilerplate or an echoed error code with the details field.
        if (Boilerplate.IsMatch(text) || (text.Length > 0 && text == code))
        {
            var d = details.Trim();
            if (d.StartsWith("Dna ", StringComparison.Ordinal))
            {
                d = d.Substring(4).Trim();
            }
            text = d.Length > 0 && !Boilerplate.IsMatch(d) && d != code
                ? d
                : "The operation could not be completed (error " + code + ").";
        }

        if (code.Length == 0)
        {
            code = "HTTP_" + status.ToString(CultureInfo.InvariantCulture);
        }
        if (text.Trim().Length == 0)
        {
            text = "HTTP " + status.ToString(CultureInfo.InvariantCulture) + " " + ReasonPhrase(status) + ".";
        }

        return Create(request, KindOf(status), code, text, status, correlationId, body,
            details: details.Length > 0 ? details : null, validationErrors: validationErrors);
    }

    public static DomainNameApiException Network(BaseRequest request, Exception exception, bool timeout, TimeSpan timeoutValue)
    {
        var message = timeout
            ? "No answer from the API within " + timeoutValue.TotalSeconds.ToString("0.##", CultureInfo.InvariantCulture) + " s."
            : "The request could not be completed: " + exception.Message;
        return Create(request, timeout ? ApiErrorKind.Timeout : ApiErrorKind.Network, timeout ? "TIMEOUT" : "NETWORK",
            message, status: null, correlationId: null, body: null, inner: exception);
    }

    public static ApiErrorKind KindOf(int status) => status switch
    {
        401 or 301 or 302 => ApiErrorKind.Authentication,
        403 => ApiErrorKind.Forbidden,
        404 => ApiErrorKind.NotFound,
        409 => ApiErrorKind.Conflict,
        429 => ApiErrorKind.RateLimited,
        400 or 422 => ApiErrorKind.Validation,
        >= 500 => ApiErrorKind.Server,
        _ => ApiErrorKind.Unknown,
    };

    /// <summary>Reads Retry-After as seconds or as an HTTP date.</summary>
    public static TimeSpan? RetryAfter(IReadOnlyDictionary<string, string> headers)
    {
        if (!headers.TryGetValue("Retry-After", out var value) || string.IsNullOrWhiteSpace(value))
        {
            return null;
        }
        value = value.Trim();
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds) && seconds >= 0)
        {
            return TimeSpan.FromSeconds(seconds);
        }
        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
        {
            var wait = date - DateTimeOffset.UtcNow;
            return wait > TimeSpan.Zero ? wait : TimeSpan.Zero;
        }
        return null;
    }

    public static string? HintFor(ApiErrorKind kind, string code, int validationErrorCount, string? correlationId, OperationSafety? safety)
    {
        switch (code)
        {
            case "INVALID_RESELLER":
                return "The reseller ID is unknown, inactive or suspended in this environment. Check ResellerId (a UUID from the panel's API settings) and Environment (Production or Ote).";
            case "INVALID_API_KEY":
                return "Production and OTE use different API keys. Make sure the key belongs to the selected Environment.";
            case "MISSING_API_KEY":
            case "MISSING_RESELLER_ID":
            case "NO_CREDENTIALS":
                return "Set ResellerId and ApiKey in DomainNameApiOptions, or the DNA_RESELLER_ID and DNA_API_KEY environment variables.";
            case "SUBRESELLER_NOT_ALLOWED":
                return "Sub-reseller accounts cannot use the API. Use the main reseller's credentials.";
            case "RATE_LIMITED":
                return "The gateway allows 5 requests/second per key (2/second for domain searches). Slow down, or keep PaceRequests on.";
            case "READ_ONLY":
                return "ReadOnly is on in DomainNameApiOptions (or DNA_READ_ONLY is set). Turn it off to send changes.";
        }

        switch (kind)
        {
            case ApiErrorKind.NotFound:
                return "Check the spelling and the environment: Production and OTE have separate data.";
            case ApiErrorKind.Validation when validationErrorCount > 0:
                return "Fix the listed fields and send the request again.";
            case ApiErrorKind.Server when correlationId is not null:
                return "Server-side failure. If it persists, contact support and quote correlation ID " + correlationId + ".";
            case ApiErrorKind.Network or ApiErrorKind.Timeout when safety is not null and not OperationSafety.Read:
                return "The request may have reached the API. Check the current state before sending it again.";
            case ApiErrorKind.Network or ApiErrorKind.Timeout:
                return "Check the network, proxy and firewall. Reads are retried automatically.";
        }
        return null;
    }

    internal static DomainNameApiException Create(
        BaseRequest request,
        ApiErrorKind kind,
        string code,
        string message,
        int? status,
        string? correlationId,
        string? body,
        string? details = null,
        IReadOnlyList<ApiValidationError>? validationErrors = null,
        TimeSpan? retryAfter = null,
        Exception? inner = null)
    {
        validationErrors ??= [];
        return new DomainNameApiException(kind, code, message, inner)
        {
            StatusCode = status,
            Details = details,
            ValidationErrors = validationErrors,
            CorrelationId = correlationId,
            RetryAfter = retryAfter,
            ResponseBody = string.IsNullOrEmpty(body) ? null : body,
            Operation = request.ToString(),
            Safety = request.Safety,
            Hint = HintFor(kind, code, validationErrors.Count, correlationId, request.Safety),
        };
    }

    private static bool TryReadJson(string text, out JsonElement root)
    {
        try
        {
            using var document = JsonDocument.Parse(text);
            root = document.RootElement.Clone();
            return root.ValueKind == JsonValueKind.Object;
        }
        catch (JsonException)
        {
            root = default;
            return false;
        }
    }

    private static string ReadString(JsonElement element, string name)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return property.Value.ValueKind switch
                {
                    JsonValueKind.String => property.Value.GetString() ?? "",
                    JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False => property.Value.GetRawText(),
                    _ => "",
                };
            }
        }
        return "";
    }

    private static string ReasonPhrase(int status) => status switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        405 => "Method Not Allowed",
        409 => "Conflict",
        422 => "Unprocessable Entity",
        500 => "Internal Server Error",
        501 => "Not Implemented",
        502 => "Bad Gateway",
        503 => "Service Unavailable",
        504 => "Gateway Timeout",
        _ => "Unexpected Status",
    };
}
