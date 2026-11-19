using System.Text.Json;
using System.Text.Json.Serialization;
using DomainNameApi.Contacts;

namespace DomainNameApi.Domains;

/// <summary>
/// The full details of a domain: dates, state, lock and privacy, nameservers, child nameservers,
/// contacts, auth code and registry attributes.
/// </summary>
public sealed record DomainDetails
{
    /// <summary>The domain name.</summary>
    public string? DomainName { get; init; }

    /// <summary>The Unicode form of an internationalised domain name.</summary>
    public string? DomainNameIdn { get; init; }

    /// <summary>Whether the domain is an internationalised domain name.</summary>
    [JsonPropertyName("isIdnName")]
    public bool IsIdn { get; init; }

    /// <summary>The state, as text (for example <c>Active</c>).</summary>
    public string? Status { get; init; }

    /// <summary>Why the domain is in its state, when the API says.</summary>
    public string? StatusReason { get; init; }

    /// <summary>The registry's status code.</summary>
    public string? StatusCode { get; init; }

    /// <summary>The redemption grace period status.</summary>
    [JsonPropertyName("rgpStatues")]
    public string? RgpStatus { get; init; }

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

    /// <summary>The reseller that holds the domain.</summary>
    public Guid ResellerId { get; init; }

    /// <summary>How the domain is renewed.</summary>
    public string? RenewalMode { get; init; }

    /// <summary>The transfer (EPP) code.</summary>
    public string? AuthCode { get; init; }

    /// <summary>Whether the transfer lock is on.</summary>
    [JsonPropertyName("lockStatus")]
    public bool IsLocked { get; init; }

    /// <summary>Whether WHOIS privacy protection is on.</summary>
    [JsonPropertyName("privacyProtectionStatus")]
    public bool IsPrivacyProtected { get; init; }

    /// <summary>Whether URL forwarding is set up.</summary>
    public bool IsForwarded { get; init; }

    /// <summary>Whether a trustee contact is used.</summary>
    public bool UseTrusteeContact { get; init; }

    /// <summary>The registry object ID.</summary>
    public string? ObjectId { get; init; }

    /// <summary>The nameservers.</summary>
    [JsonPropertyName("nameservers")]
    public IReadOnlyList<string> NameServers { get; init; } = [];

    /// <summary>The child nameservers (glue records) of the domain.</summary>
    public IReadOnlyList<DomainHost> Hosts { get; init; } = [];

    /// <summary>The contacts, one per role.</summary>
    public IReadOnlyList<Contact> Contacts { get; init; } = [];

    /// <summary>Registry-specific attributes, such as those of .tr domains.</summary>
    public IReadOnlyDictionary<string, JsonElement>? AdditionalAttributes { get; init; }

    /// <summary>Returns the contact with the given role, or null.</summary>
    /// <param name="role">Registrant, Admin, Tech or Billing.</param>
    public Contact? GetContact(ContactType role) => Contacts.FirstOrDefault(c => c.ContactType == role);
}
