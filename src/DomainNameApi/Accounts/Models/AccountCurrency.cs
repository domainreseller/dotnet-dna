namespace DomainNameApi.Accounts;

/// <summary>The currency a reseller account is billed in.</summary>
public enum AccountCurrency
{
    /// <summary>Turkish lira (TRY).</summary>
    Try = 1,

    /// <summary>US dollar (USD).</summary>
    Usd = 2,

    /// <summary>Euro (the API's <c>EURO</c>).</summary>
    Euro = 3,
}
