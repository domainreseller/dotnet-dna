namespace DomainNameApi.Domains;

/// <summary>Whether a domain can be registered, and at what price.</summary>
public sealed record DomainAvailability
{
    /// <summary>The domain name.</summary>
    public string? DomainName { get; init; }

    /// <summary>The TLD, for example <c>COM</c>.</summary>
    public string? Tld { get; init; }

    /// <summary>The availability as the API reports it: <c>AVAILABLE</c> or <c>NOTAVAILABLE</c>.</summary>
    public string? Status { get; init; }

    /// <summary>Whether the domain can be registered.</summary>
    public bool IsAvailable => string.Equals(Status, "AVAILABLE", StringComparison.OrdinalIgnoreCase);

    /// <summary>Why the domain is not available, for example <c>Domain exists</c>.</summary>
    public string? Reason { get; init; }

    /// <summary>Whether the domain is a premium name with a registry-set price.</summary>
    public bool? IsPremium { get; init; }

    /// <summary>Whether the registry requires documents for this domain (common for .tr).</summary>
    public bool IsDocumentRequired { get; init; }

    /// <summary>The registration price for <see cref="Period"/> years.</summary>
    public decimal? Price { get; init; }

    /// <summary>The currency of <see cref="Price"/>.</summary>
    public string? Currency { get; init; }

    /// <summary>The period in years the price is for.</summary>
    public int? Period { get; init; }
}
