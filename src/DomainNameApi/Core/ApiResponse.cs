using System.Globalization;

namespace DomainNameApi;

/// <summary>
/// Details of a completed HTTP exchange: status, correlation ID, quota and timing.
/// Returned by <see cref="DomainNameApiClient.SendWithResponseAsync(BaseRequest, CancellationToken)"/>.
/// </summary>
public class ApiResponse
{
    internal ApiResponse(int statusCode, IReadOnlyDictionary<string, string> headers, int attempts, TimeSpan duration)
    {
        StatusCode = statusCode;
        Headers = headers;
        Attempts = attempts;
        Duration = duration;
        CorrelationId = headers.TryGetValue("X-Correlation-Id", out var id) && id.Length > 0 ? id : null;
        Quota = ApiQuota.FromHeaders(headers);
    }

    /// <summary>The HTTP status code.</summary>
    public int StatusCode { get; }

    /// <summary>The gateway's <c>X-Correlation-Id</c>. Quote it when contacting support.</summary>
    public string? CorrelationId { get; }

    /// <summary>The soft-quota state after this request, or null when the gateway sent none.</summary>
    public ApiQuota? Quota { get; }

    /// <summary>How many times the request was sent (1 when it was not retried).</summary>
    public int Attempts { get; }

    /// <summary>Total time including retries and pacing.</summary>
    public TimeSpan Duration { get; }

    /// <summary>The response headers (case-insensitive names).</summary>
    public IReadOnlyDictionary<string, string> Headers { get; }
}

/// <summary>An <see cref="ApiResponse"/> together with the decoded body.</summary>
/// <typeparam name="T">The type of the body.</typeparam>
public sealed class ApiResponse<T> : ApiResponse
{
    internal ApiResponse(T value, ApiResponse response)
        : base(response.StatusCode, response.Headers, response.Attempts, response.Duration)
    {
        Value = value;
    }

    /// <summary>The decoded response body.</summary>
    public T Value { get; }
}

/// <summary>
/// The gateway's soft quota. It never rejects requests: once <see cref="Used"/> passes <see cref="Limit"/>,
/// each request is held for up to 5 seconds until points decay (20% per minute). Searches and info
/// lookups cost 1 point, everything else 10.
/// </summary>
public sealed record ApiQuota
{
    /// <summary>The quota category: domain, contact, ssl, products, order, deposit or hosting.</summary>
    public string? Scope { get; init; }

    /// <summary>Points charged for this request.</summary>
    public double Cost { get; init; }

    /// <summary>Points in the bucket after this request.</summary>
    public double Used { get; init; }

    /// <summary>The threshold where slowing down starts.</summary>
    public double Limit { get; init; }

    /// <summary>Seconds the gateway held this request.</summary>
    public double DelaySeconds { get; init; }

    /// <summary>Seconds until the bucket is empty again.</summary>
    public double ResetSeconds { get; init; }

    /// <summary>Whether the gateway slowed this request down.</summary>
    public bool WasDelayed => DelaySeconds > 0;

    internal static ApiQuota? FromHeaders(IReadOnlyDictionary<string, string> headers)
    {
        if (!headers.ContainsKey("X-Quota-Limit") && !headers.ContainsKey("X-Quota-Used"))
        {
            return null;
        }
        double Number(string name) =>
            headers.TryGetValue(name, out var s) && double.TryParse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var n) ? n : 0;
        headers.TryGetValue("X-Quota-Scope", out var scope);
        return new ApiQuota
        {
            Scope = scope,
            Cost = Number("X-Quota-Cost"),
            Used = Number("X-Quota-Used"),
            Limit = Number("X-Quota-Limit"),
            DelaySeconds = Number("X-Quota-Delay"),
            ResetSeconds = Number("X-Quota-Reset"),
        };
    }
}
