namespace DomainNameApi.Accounts;

/// <summary>Your reseller account and its balances.</summary>
public sealed record ResellerAccount
{
    /// <summary>Your reseller ID.</summary>
    public Guid ResellerId { get; init; }

    /// <summary>Your reseller name.</summary>
    public string? ResellerName { get; init; }

    /// <summary>The price group your account belongs to.</summary>
    public string? ResellerGroupName { get; init; }

    /// <summary>Balance in Turkish lira.</summary>
    public decimal TryBalance { get; init; }

    /// <summary>Balance in US dollars.</summary>
    public decimal UsdBalance { get; init; }
}
