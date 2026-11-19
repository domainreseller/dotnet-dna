namespace DomainNameApi.Domains;

/// <summary>A registry document of a domain.</summary>
public sealed record DomainDocument
{
    /// <summary>The file name.</summary>
    public string? Name { get; init; }

    /// <summary>What the document is.</summary>
    public DocumentType Type { get; init; }

    /// <summary>The review state.</summary>
    public DocumentStatus Status { get; init; }

    /// <summary>The operation the document was requested for.</summary>
    public DocumentOperationType OperationType { get; init; }

    /// <summary>When the document was uploaded.</summary>
    public DateTime UploadedDate { get; init; }
}
