namespace DomainNameApi.Accounts;

/// <summary>The state of a reseller account.</summary>
public enum ResellerStatus
{
    /// <summary>Active.</summary>
    Active = 0,

    /// <summary>Waiting for approval.</summary>
    WaitingForApproval = 1,

    /// <summary>Suspended.</summary>
    Suspend = 2,

    /// <summary>Waiting for the sign-up wizard to be completed.</summary>
    WaitingForCompletionWizard = 3,
}
