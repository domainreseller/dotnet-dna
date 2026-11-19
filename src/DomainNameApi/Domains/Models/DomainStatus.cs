namespace DomainNameApi.Domains;

/// <summary>The lifecycle state of a domain.</summary>
public enum DomainStatus
{
    /// <summary>Registered and working.</summary>
    Active = 0,

    /// <summary>The registration is being processed.</summary>
    WaitingForRegistration = 1,

    /// <summary>Registry documents must be uploaded.</summary>
    WaitingForDocument = 2,

    /// <summary>An internal transfer from another reseller is pending.</summary>
    WaitingForIncomingTransfer = 3,

    /// <summary>Transferred to another registrar.</summary>
    TransferredOut = 4,

    /// <summary>The transfer was rejected.</summary>
    TransferRejected = 5,

    /// <summary>About to be deleted.</summary>
    PendingDelete = 7,

    /// <summary>Deleted.</summary>
    Deleted = 8,

    /// <summary>The transfer confirmation e-mail was sent.</summary>
    ConfirmationEmailSend = 9,

    /// <summary>An outgoing transfer is pending.</summary>
    WaitingForOutgoingTransfer = 11,

    /// <summary>On hold.</summary>
    PendingHold = 12,

    /// <summary>A change is being processed.</summary>
    ModificationPending = 15,

    /// <summary>Subject to a legal process.</summary>
    InCase = 18,

    /// <summary>A restore from redemption is pending.</summary>
    PendingRestore = 19,
}
