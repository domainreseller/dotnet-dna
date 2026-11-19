namespace DomainNameApi.Contacts;

/// <summary>The result of updating a contact handle, including the domains that use it.</summary>
public sealed record HandleUpdateResult
{
    /// <summary>The contact handle.</summary>
    public string? Handle { get; init; }

    /// <summary>Whether the contact itself was updated.</summary>
    public bool ContactUpdateSuccess { get; init; }

    /// <summary>Counts of the affected domains.</summary>
    public HandleUpdateStats? Stats { get; init; }

    /// <summary>What happened to each domain that uses the handle.</summary>
    public IReadOnlyList<HandleDomainUpdate> AffectedDomains { get; init; } = [];
}

/// <summary>Counts of the domains affected by a contact update.</summary>
public sealed record HandleUpdateStats
{
    /// <summary>Domains that use the handle.</summary>
    public int TotalDomainCount { get; init; }

    /// <summary>Domains updated.</summary>
    public int SuccessCount { get; init; }

    /// <summary>Domains waiting for registry documents.</summary>
    public int PendingDocumentCount { get; init; }

    /// <summary>Domains that could not be updated.</summary>
    public int FailedCount { get; init; }

    /// <summary>Domains that received a new handle because the change could not apply to them.</summary>
    public int SplitCount { get; init; }
}

/// <summary>What happened to one domain when its contact handle was updated.</summary>
public sealed record HandleDomainUpdate
{
    /// <summary>The domain.</summary>
    public string? DomainName { get; init; }

    /// <summary>The outcome for this domain.</summary>
    public string? Status { get; init; }

    /// <summary>The new handle, when the domain was given one.</summary>
    public string? NewHandle { get; init; }

    /// <summary>Why the domain got this outcome.</summary>
    public string? Reason { get; init; }

    /// <summary>The API's message.</summary>
    public string? Message { get; init; }
}
