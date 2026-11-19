namespace DomainNameApi.Domains;

/// <summary>The review state of a registry document.</summary>
public enum DocumentStatus
{
    /// <summary>Expected but not sent yet.</summary>
    Waiting = 1,

    /// <summary>Sent, not reviewed yet.</summary>
    Pending = 2,

    /// <summary>Approved.</summary>
    Approved = 3,

    /// <summary>Rejected.</summary>
    Rejected = 4,

    /// <summary>Expired.</summary>
    Expired = 5,

    /// <summary>The application or owner change was cancelled.</summary>
    Cancelled = 6,
}
