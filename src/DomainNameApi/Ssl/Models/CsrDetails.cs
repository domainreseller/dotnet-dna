namespace DomainNameApi.Ssl;

/// <summary>The subject of a decoded CSR.</summary>
public sealed record CsrDetails
{
    /// <summary>The common name (domain).</summary>
    public string? DomainName { get; init; }

    /// <summary>Two-letter country code.</summary>
    public string? Country { get; init; }

    /// <summary>State or province.</summary>
    public string? State { get; init; }

    /// <summary>City.</summary>
    public string? Locality { get; init; }

    /// <summary>Organisation.</summary>
    public string? Organization { get; init; }

    /// <summary>Organisational unit.</summary>
    public string? OrganizationUnit { get; init; }

    /// <summary>The subject alternative names.</summary>
    public string? AlternativeNames { get; init; }

    /// <summary>Whether the CSR is for a wildcard name.</summary>
    public bool IsWildcard { get; init; }

    /// <summary>Whether the CSR has subject alternative names.</summary>
    public bool IsSan { get; init; }
}
