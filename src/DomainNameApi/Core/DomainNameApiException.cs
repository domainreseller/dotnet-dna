namespace DomainNameApi;

/// <summary>A coarse, stable classification of failures. Branch on it instead of parsing messages.</summary>
public enum ApiErrorKind
{
    /// <summary>Anything that fits no other kind, including an answer the SDK could not read.</summary>
    Unknown,

    /// <summary>Missing or wrong credentials, or an unknown, inactive or suspended reseller (HTTP 401, or a redirect).</summary>
    Authentication,

    /// <summary>Authenticated but not allowed (HTTP 403).</summary>
    Forbidden,

    /// <summary>The domain, contact, order or other resource does not exist (HTTP 404).</summary>
    NotFound,

    /// <summary>The API refused the input or a business rule failed: bad field, domain locked, not renewable, insufficient balance (HTTP 400/422).</summary>
    Validation,

    /// <summary>The resource is in a state that does not allow the operation (HTTP 409).</summary>
    Conflict,

    /// <summary>Still rate limited after the automatic retries (HTTP 429).</summary>
    RateLimited,

    /// <summary>A server-side failure (HTTP 5xx). Often temporary.</summary>
    Server,

    /// <summary>The request could not be completed on the network: DNS, TLS, proxy, connection reset.</summary>
    Network,

    /// <summary>No answer within <see cref="DomainNameApiOptions.Timeout"/>.</summary>
    Timeout,

    /// <summary>Blocked by <see cref="DomainNameApiOptions.ReadOnly"/>; nothing was sent.</summary>
    ReadOnly,
}

/// <summary>One field-level problem reported by the API.</summary>
public sealed record ApiValidationError
{
    /// <summary>What is wrong.</summary>
    public string Message { get; init; } = "";

    /// <summary>The fields the problem refers to, for example <c>contacts[0].eMail</c>.</summary>
    public IReadOnlyList<string> Members { get; init; } = [];

    /// <inheritdoc />
    public override string ToString() => Members.Count > 0 ? string.Join(", ", Members) + ": " + Message : Message;
}

/// <summary>
/// Thrown for every failed request: API errors (non-2xx answers), network failures, timeouts and
/// read-only blocks. Invalid arguments throw <see cref="ArgumentException"/> instead, before anything is sent.
/// </summary>
/// <example>
/// <code>
/// try
/// {
///     await client.Domains.RenewAsync("example.com", years: 1);
/// }
/// catch (DomainNameApiException e) when (e.Kind == ApiErrorKind.Validation)
/// {
///     Console.WriteLine($"{e.Code}: {e.Message}");
/// }
/// </code>
/// </example>
public class DomainNameApiException : Exception
{
    /// <summary>Creates an exception.</summary>
    /// <param name="kind">The classification.</param>
    /// <param name="code">The API's error code, or one of the SDK's codes such as <c>INVALID_API_KEY</c>.</param>
    /// <param name="message">What happened.</param>
    /// <param name="innerException">The underlying exception, if any.</param>
    public DomainNameApiException(ApiErrorKind kind, string code, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Kind = kind;
        Code = code;
    }

    /// <summary>The classification of the failure.</summary>
    public ApiErrorKind Kind { get; }

    /// <summary>
    /// The API's error code when it sends one (for example <c>Dna.DomainService:Domain:10007</c>). These come
    /// from the platform's error catalogue and identify the exact reason. Otherwise one of the SDK's codes:
    /// <c>INVALID_RESELLER</c>, <c>INVALID_API_KEY</c>, <c>MISSING_API_KEY</c>, <c>MISSING_RESELLER_ID</c>,
    /// <c>SUBRESELLER_NOT_ALLOWED</c>, <c>UNAUTHORIZED</c>, <c>NO_CREDENTIALS</c>, <c>RATE_LIMITED</c>,
    /// <c>NETWORK</c>, <c>TIMEOUT</c>, <c>READ_ONLY</c>, <c>EMPTY_RESPONSE</c>, <c>INVALID_RESPONSE</c> or <c>HTTP_&lt;status&gt;</c>.
    /// </summary>
    public string Code { get; }

    /// <summary>The HTTP status, or null when no answer was received.</summary>
    public int? StatusCode { get; init; }

    /// <summary>Additional detail the API sent with the error.</summary>
    public string? Details { get; init; }

    /// <summary>Field-level problems the API reported.</summary>
    public IReadOnlyList<ApiValidationError> ValidationErrors { get; init; } = [];

    /// <summary>The gateway's correlation ID. Quote it when contacting support.</summary>
    public string? CorrelationId { get; init; }

    /// <summary>How long the gateway asked to wait before retrying (rate limits).</summary>
    public TimeSpan? RetryAfter { get; init; }

    /// <summary>The raw response body, when there was one.</summary>
    public string? ResponseBody { get; init; }

    /// <summary>The request that failed, for example <c>POST domains/renew</c>.</summary>
    public string? Operation { get; init; }

    /// <summary>What the failed request could do to your account.</summary>
    public OperationSafety? Safety { get; init; }

    /// <summary>A short suggestion on what to do next, or null.</summary>
    public string? Hint { get; init; }

    /// <summary>Whether repeating the same request later can succeed.</summary>
    public bool IsRetryable =>
        Kind is ApiErrorKind.RateLimited or ApiErrorKind.Network or ApiErrorKind.Timeout ||
        (Kind == ApiErrorKind.Server && StatusCode != 501);

    /// <summary>
    /// True when the outcome is unknown: a request that changes something failed on the network or timed
    /// out, so the API may have carried it out. Check the state (for example with
    /// <c>Domains.GetAsync</c> or the account transactions) before sending it again.
    /// </summary>
    public bool OutcomeUnknown =>
        Kind is ApiErrorKind.Network or ApiErrorKind.Timeout && Safety is not null and not OperationSafety.Read;

    /// <inheritdoc />
    public override string ToString()
    {
        var text = GetType().FullName + ": [" + Code + "] " + Message;
        if (StatusCode is not null)
        {
            text += " (HTTP " + StatusCode + ")";
        }
        foreach (var error in ValidationErrors)
        {
            text += System.Environment.NewLine + "  - " + error;
        }
        if (Hint is not null)
        {
            text += System.Environment.NewLine + "Hint: " + Hint;
        }
        if (CorrelationId is not null)
        {
            text += System.Environment.NewLine + "Correlation ID: " + CorrelationId;
        }
        if (InnerException is not null)
        {
            text += System.Environment.NewLine + " ---> " + InnerException;
        }
        return text + System.Environment.NewLine + StackTrace;
    }
}
