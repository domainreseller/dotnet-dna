namespace DomainNameApi.Accounts;

/// <summary>Whether a transaction added money to the balance or took it.</summary>
public enum ChargeType
{
    /// <summary>Credit: money added.</summary>
    Add = 0,

    /// <summary>Debit: money taken.</summary>
    Subtract = 1,
}
