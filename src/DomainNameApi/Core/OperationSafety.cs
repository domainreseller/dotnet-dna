namespace DomainNameApi;

/// <summary>
/// What an API operation can do to your account. Every request declares one, so you can
/// tell a harmless lookup from a call that spends money before you send it.
/// </summary>
public enum OperationSafety
{
    /// <summary>No side effects, even when the API uses POST or PUT (for example a domain search).</summary>
    Read,

    /// <summary>A free change that can be undone (DNS records, locks, contacts, nameservers).</summary>
    Write,

    /// <summary>Charges your balance: registrations, renewals, transfers, restores, SSL and hosting orders.</summary>
    Billable,

    /// <summary>Deletes something or is hard to undo: record deletion, DNSSEC removal, certificate revocation.</summary>
    Destructive,
}
