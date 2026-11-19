using System.Text.Json;
using System.Text.Json.Serialization;

namespace DomainNameApi.Domains;

/// <summary>Whether a domain can be transferred in with a given auth code.</summary>
public sealed record TransferCheck
{
    /// <summary>Whether the domain can be transferred.</summary>
    [JsonPropertyName("transferAvailabilityStatus")]
    public bool IsTransferable { get; init; }

    /// <summary>Whether the registry needs an auth code.</summary>
    [JsonPropertyName("authCodeIsRequired")]
    public bool IsAuthCodeRequired { get; init; }

    /// <summary>Whether the given auth code is valid.</summary>
    [JsonPropertyName("authCodeIsValid")]
    public bool IsAuthCodeValid { get; init; }

    /// <summary>Whether the registrant has to approve the transfer.</summary>
    [JsonPropertyName("userTransferRequired")]
    public bool IsUserApprovalRequired { get; init; }

    /// <summary>Whether a transfer lock is on at the current registrar.</summary>
    [JsonPropertyName("transferLock")]
    public bool HasTransferLock { get; init; }

    /// <summary>The registry statuses of the domain.</summary>
    public IReadOnlyList<string> DomainStatuses { get; init; } = [];

    /// <summary>A key identifying the message.</summary>
    public string? MessageKey { get; init; }

    /// <summary>The API's message.</summary>
    public string? Message { get; init; }

    /// <summary>Whether the domain is a premium name.</summary>
    public bool IsPremium { get; init; }

    /// <summary>The premium transfer price.</summary>
    public decimal? PremiumPrice { get; init; }

    /// <summary>The currency of <see cref="PremiumPrice"/>.</summary>
    public string? PremiumCurrency { get; init; }

    /// <summary>Registry-specific attributes.</summary>
    public IReadOnlyDictionary<string, JsonElement>? AdditionalAttributes { get; init; }
}
