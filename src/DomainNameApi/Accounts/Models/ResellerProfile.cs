namespace DomainNameApi.Accounts;

/// <summary>Your reseller profile: who the account is, its currency and its state.</summary>
public sealed record ResellerProfile
{
    /// <summary>Your reseller ID.</summary>
    public Guid Id { get; init; }

    /// <summary>The numeric ID of your reseller account.</summary>
    public int ResellerAutoId { get; init; }

    /// <summary>Your reseller name.</summary>
    public string? Name { get; init; }

    /// <summary>Your company name.</summary>
    public string? CompanyName { get; init; }

    /// <summary>The price group your account belongs to.</summary>
    public string? ResellerGroupName { get; init; }

    /// <summary>The currency your account is billed in.</summary>
    public AccountCurrency AccountCurrency { get; init; }

    /// <summary>The state of the account.</summary>
    public ResellerStatus Status { get; init; }

    /// <summary>Whether the account is a sub-reseller of another reseller.</summary>
    public bool IsSubReseller { get; init; }

    /// <summary>Whether the balance is suspended. A suspended balance cannot be charged.</summary>
    public bool IsBalanceSuspended { get; init; }

    /// <summary>Whether MCP access is enabled for the account.</summary>
    public bool IsMcpEnabled { get; init; }
}
