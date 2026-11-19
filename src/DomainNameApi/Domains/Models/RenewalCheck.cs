using System.Text.Json.Serialization;

namespace DomainNameApi.Domains;

/// <summary>Whether a domain can be renewed, and at what price if it is premium.</summary>
public sealed record RenewalCheck
{
    /// <summary>Whether the domain can be renewed.</summary>
    [JsonPropertyName("renewabilityStatus")]
    public bool IsRenewable { get; init; }

    /// <summary>Whether a renewal lock prevents renewing.</summary>
    [JsonPropertyName("renewalLock")]
    public bool HasRenewalLock { get; init; }

    /// <summary>The reason when it cannot be renewed.</summary>
    public string? ErrorMessageKey { get; init; }

    /// <summary>Whether the domain is a premium name.</summary>
    public bool IsPremium { get; init; }

    /// <summary>The premium renewal price.</summary>
    public decimal? PremiumPrice { get; init; }

    /// <summary>The currency of <see cref="PremiumPrice"/>.</summary>
    public string? PremiumCurrency { get; init; }
}
