namespace DomainNameApi.Accounts;

/// <summary>What a balance transaction was for.</summary>
public enum TransactionType
{
    /// <summary>Domain registration.</summary>
    Register = 1,

    /// <summary>Renewal.</summary>
    Renew = 2,

    /// <summary>Transfer.</summary>
    Transfer = 3,

    /// <summary>Restore from redemption.</summary>
    Restore = 4,

    /// <summary>Balance deposit.</summary>
    Deposit = 5,

    /// <summary>Backorder.</summary>
    Backorder = 6,

    /// <summary>Refund.</summary>
    Refund = 7,

    /// <summary>Deduction.</summary>
    Deduction = 8,

    /// <summary>Pre-registration.</summary>
    PreRegister = 9,

    /// <summary>Purchase (SSL, hosting and other products).</summary>
    Shopping = 10,

    /// <summary>Payment between resellers.</summary>
    ResellerPayment = 11,

    /// <summary>Transfer between resellers on the platform.</summary>
    InternalTransfer = 12,
}
