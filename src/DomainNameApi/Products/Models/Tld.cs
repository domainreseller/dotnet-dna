using System.Text.Json.Serialization;

namespace DomainNameApi.Products;

/// <summary>A TLD with its prices, periods, rules and required registration attributes.</summary>
public sealed record Tld
{
    /// <summary>The TLD without a leading dot, for example <c>com</c> or <c>com.tr</c>.</summary>
    public string? Name { get; init; }

    /// <summary>The shortest registration, in years.</summary>
    public int MinRegistrationPeriod { get; init; }

    /// <summary>The longest registration, in years.</summary>
    public int MaxRegistrationPeriod { get; init; }

    /// <summary>The registration periods offered, in years.</summary>
    public IReadOnlyList<int> RegistrationPeriods { get; init; } = [];

    /// <summary>The renewal periods offered, in years.</summary>
    public IReadOnlyList<int> RenewalPeriods { get; init; } = [];

    /// <summary>Prices per price group: register, renew, transfer, restore.</summary>
    public IReadOnlyList<TldPriceGroup> Prices { get; init; } = [];

    /// <summary>Attributes the registry needs at registration, passed as <c>TldAttributes</c>.</summary>
    public IReadOnlyList<TldAttribute> Attributes { get; init; } = [];

    /// <summary>Whether domains can be transferred in.</summary>
    [JsonPropertyName("isSupportTransfer")]
    public bool SupportsTransfer { get; init; }

    /// <summary>Whether a transfer can be checked before it is sent.</summary>
    [JsonPropertyName("isTransferCheckSupported")]
    public bool SupportsTransferCheck { get; init; }

    /// <summary>Whether transfers need an auth code.</summary>
    [JsonPropertyName("isRequiredAuthCode")]
    public bool RequiresAuthCode { get; init; }

    /// <summary>Whether the transfer lock can be set.</summary>
    [JsonPropertyName("isTransferLockPossible")]
    public bool SupportsTransferLock { get; init; }

    /// <summary>Whether internationalised names are supported.</summary>
    [JsonPropertyName("isSupportIdn")]
    public bool SupportsIdn { get; init; }

    /// <summary>Whether the registry asks for documents.</summary>
    [JsonPropertyName("isRequireDocument")]
    public bool RequiresDocument { get; init; }

    /// <summary>Whether a trustee (local presence) service is offered.</summary>
    [JsonPropertyName("isSupportTrustee")]
    public bool SupportsTrustee { get; init; }

    /// <summary>Whether a trustee is required.</summary>
    [JsonPropertyName("isTrusteeRequired")]
    public bool RequiresTrustee { get; init; }

    /// <summary>Whether the TLD is on sale.</summary>
    public bool IsSaleAllowed { get; init; }

    /// <summary>Whether contacts can be changed through handles.</summary>
    [JsonPropertyName("isHandleUpdateAvailable")]
    public bool SupportsHandleUpdate { get; init; }

    /// <summary>Whether WHOIS privacy is offered.</summary>
    [JsonPropertyName("isPrivacyAvailable")]
    public bool SupportsPrivacy { get; init; }

    /// <summary>Whether DNSSEC is supported.</summary>
    [JsonPropertyName("isDnssecAvailable")]
    public bool SupportsDnssec { get; init; }

    /// <summary>Whether IPv6 glue records are accepted.</summary>
    [JsonPropertyName("isIpv6Available")]
    public bool SupportsIpv6 { get; init; }

    /// <summary>Whether the registry supports explicit renewals.</summary>
    [JsonPropertyName("isExplicitRenewalSupported")]
    public bool SupportsExplicitRenewal { get; init; }

    /// <summary>Whether glue records are allowed.</summary>
    [JsonPropertyName("isGlueRecordAllowed")]
    public bool AllowsGlueRecords { get; init; }

    /// <summary>Whether the registry uses host objects for nameservers.</summary>
    [JsonPropertyName("isHostObjectAccepted")]
    public bool AcceptsHostObjects { get; init; }

    /// <summary>Whether the registry checks the nameservers' DNS.</summary>
    [JsonPropertyName("registryDnsCheck")]
    public bool HasRegistryDnsCheck { get; init; }

    /// <summary>Whether the TLD can be used in the OTE test environment.</summary>
    [JsonPropertyName("availableInTestEnvironment")]
    public bool IsAvailableInTestEnvironment { get; init; }

    /// <summary>Add grace period, in days.</summary>
    public int AddGracePeriod { get; init; }

    /// <summary>Auto-renew grace period, in days.</summary>
    public int AutoRenewGracePeriod { get; init; }

    /// <summary>Finalisation period, in days.</summary>
    public int FinalizationPeriod { get; init; }

    /// <summary>Failure period, in days.</summary>
    public int FailurePeriod { get; init; }

    /// <summary>Payment period, in days.</summary>
    public int PaymentPeriod { get; init; }

    /// <summary>How long a deleted domain can be restored, in days.</summary>
    public int DeletionRestorablePeriod { get; init; }

    /// <summary>How long a deleted domain is held, in days.</summary>
    public int DeletionHoldPeriod { get; init; }

    /// <summary>Transfer period, in days.</summary>
    public int TransferPeriod { get; init; }

    /// <summary>Register period, in days.</summary>
    public int RegisterPeriod { get; init; }

    /// <summary>Renewal period, in days.</summary>
    public int RenewalPeriod { get; init; }

    /// <summary>Returns the registration price for a period from the first price group, or null.</summary>
    /// <param name="years">The period in years.</param>
    public TldPrice? GetRegistrationPrice(int years = 1) =>
        Prices.SelectMany(g => g.Register).FirstOrDefault(p => p.Period == years);

    /// <summary>Returns the renewal price for a period from the first price group, or null.</summary>
    /// <param name="years">The period in years.</param>
    public TldPrice? GetRenewalPrice(int years = 1) =>
        Prices.SelectMany(g => g.Renew).FirstOrDefault(p => p.Period == years);
}

/// <summary>The prices of a TLD in one price group.</summary>
public sealed record TldPriceGroup
{
    /// <summary>The price group.</summary>
    public string? PriceGroup { get; init; }

    /// <summary>Registration prices per period.</summary>
    public IReadOnlyList<TldPrice> Register { get; init; } = [];

    /// <summary>Renewal prices per period.</summary>
    public IReadOnlyList<TldPrice> Renew { get; init; } = [];

    /// <summary>Transfer prices per period.</summary>
    public IReadOnlyList<TldPrice> Transfer { get; init; } = [];

    /// <summary>The restore price.</summary>
    public TldPrice? Restore { get; init; }

    /// <summary>The refund amount.</summary>
    public TldPrice? Refund { get; init; }

    /// <summary>The backorder price.</summary>
    public TldPrice? Backorder { get; init; }
}

/// <summary>A price for a period.</summary>
public sealed record TldPrice
{
    /// <summary>The period in years.</summary>
    public int Period { get; init; }

    /// <summary>The price.</summary>
    public decimal Price { get; init; }

    /// <summary>The currency.</summary>
    public string? Currency { get; init; }
}

/// <summary>An attribute a registry needs at registration (for example for .tr domains).</summary>
public sealed record TldAttribute
{
    /// <summary>The key to use in <c>TldAttributes</c>.</summary>
    public string? Key { get; init; }

    /// <summary>What the attribute means.</summary>
    public string? Description { get; init; }

    /// <summary>Whether it must be given.</summary>
    public bool IsRequired { get; init; }

    /// <summary>The value type.</summary>
    public string? Type { get; init; }

    /// <summary>The allowed values, when the attribute is a choice.</summary>
    public IReadOnlyList<TldAttributeOption> Options { get; init; } = [];
}

/// <summary>An allowed value of a TLD attribute.</summary>
public sealed record TldAttributeOption
{
    /// <summary>The value.</summary>
    public string? Value { get; init; }
}
