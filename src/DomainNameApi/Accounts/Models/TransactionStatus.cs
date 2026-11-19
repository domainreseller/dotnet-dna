namespace DomainNameApi.Accounts;

/// <summary>The outcome of a balance transaction.</summary>
public enum TransactionStatus
{
    /// <summary>Failed.</summary>
    Failed = 0,

    /// <summary>Completed.</summary>
    Success = 1,

    /// <summary>Cancelled.</summary>
    Canceled = 2,

    /// <summary>Still being processed.</summary>
    Processing = 3,

    /// <summary>Refunded.</summary>
    Refunded = 4,
}
