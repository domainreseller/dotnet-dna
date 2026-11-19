namespace DomainNameApi.Domains;

/// <summary>The transfer (EPP) code of a domain.</summary>
public sealed record DomainAuthCode
{
    /// <summary>The domain name.</summary>
    public string? DomainName { get; init; }

    /// <summary>The auth code.</summary>
    public string? AuthCode { get; init; }
}
