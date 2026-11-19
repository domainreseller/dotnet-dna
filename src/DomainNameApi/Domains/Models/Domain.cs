using System.Text.Json;
using System.Text.Json.Serialization;

namespace DomainNameApi.Domains;

/// <summary>A domain in your account, as returned by the domain list.</summary>
public sealed record Domain
{
    /// <summary>The domain name.</summary>
    public string? DomainName { get; init; }

    /// <summary>The Unicode form of an internationalised domain name.</summary>
    public string? DomainNameIdn { get; init; }

    /// <summary>Whether the domain is an internationalised domain name.</summary>
    [JsonPropertyName("isIdnName")]
    public bool IsIdn { get; init; }

    /// <summary>The lifecycle state.</summary>
    public DomainStatus Status { get; init; }

    /// <summary>The registry's status code.</summary>
    public string? StatusCode { get; init; }

    /// <summary>When the domain was registered.</summary>
    public DateTime StartDate { get; init; }

    /// <summary>When the domain expires.</summary>
    public DateTime ExpirationDate { get; init; }

    /// <summary>Days until <see cref="ExpirationDate"/>.</summary>
    [JsonPropertyName("remainingDay")]
    public int RemainingDays { get; init; }

    /// <summary>When the domain was transferred in, if it was.</summary>
    public DateTime? TransferDate { get; init; }

    /// <summary>When the domain was last changed.</summary>
    public DateTime UpdatedDate { get; init; }

    /// <summary>End of the redemption grace period, if the domain is in it.</summary>
    public DateTime? RgpExpirationDate { get; init; }

    /// <summary>End of the add grace period.</summary>
    public DateTime? AgpExpirationDate { get; init; }

    /// <summary>The failure date the registry reports.</summary>
    public DateTime FailureDate { get; init; }

    /// <summary>The finalisation date the registry reports.</summary>
    public DateTime FinalizationDate { get; init; }

    /// <summary>Whether the transfer lock is on.</summary>
    [JsonPropertyName("lockStatus")]
    public bool IsLocked { get; init; }

    /// <summary>Whether WHOIS privacy protection is on.</summary>
    [JsonPropertyName("privacyProtectionStatus")]
    public bool IsPrivacyProtected { get; init; }

    /// <summary>How the domain is renewed.</summary>
    public string? RenewalMode { get; init; }

    /// <summary>The registry object class.</summary>
    public string? ObjectClass { get; init; }

    /// <summary>The nameservers.</summary>
    [JsonPropertyName("nameservers")]
    public IReadOnlyList<DomainNameServer> NameServers { get; init; } = [];

    /// <summary>The contacts, as the API sends them in the list.</summary>
    public JsonElement? Contacts { get; init; }
}
