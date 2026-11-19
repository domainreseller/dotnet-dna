namespace DomainNameApi;

/// <summary>Kind of order (also used to ask for a product's price).</summary>
public enum OrderType
{
    /// <summary>New registration.</summary>
    Register = 1,

    /// <summary>Renewal.</summary>
    Renew = 2,

    /// <summary>Transfer from another registrar.</summary>
    Transfer = 3,

    /// <summary>Restore from redemption.</summary>
    Restore = 4,

    /// <summary>Transfer between sub-resellers.</summary>
    SubresellerTransfer = 5,

    /// <summary>Backorder.</summary>
    Backorder = 6,

    /// <summary>Transfer between resellers on the platform.</summary>
    InternalTransfer = 7,

    /// <summary>A charge.</summary>
    Charge = 8,

    /// <summary>Trustee service.</summary>
    Trustee = 9,

    /// <summary>Leasing (hosting).</summary>
    Leasing = 10,

    /// <summary>Creation (hosting, SSL).</summary>
    Create = 11,

    /// <summary>Owner change.</summary>
    OwnerChange = 12,
}
