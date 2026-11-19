namespace DomainNameApi.Domains;

/// <summary>The operation a document was requested for.</summary>
public enum DocumentOperationType
{
    /// <summary>A new registration.</summary>
    Create = 1,

    /// <summary>A registrant change.</summary>
    OwnerChange = 2,

    /// <summary>A deletion.</summary>
    Delete = 3,
}
