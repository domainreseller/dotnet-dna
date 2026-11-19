namespace DomainNameApi.Domains;

/// <summary>The result of uploading registry documents.</summary>
public sealed record DocumentUploadResult
{
    /// <summary>The documents that were stored.</summary>
    public IReadOnlyList<UploadedDocument> Uploaded { get; init; } = [];

    /// <summary>File names that were already uploaded.</summary>
    public IReadOnlyList<string> Duplicates { get; init; } = [];

    /// <summary>File names that could not be stored.</summary>
    public IReadOnlyList<string> Failed { get; init; } = [];

    /// <summary>The number of documents the domain has now.</summary>
    public int TotalDocumentCount { get; init; }

    /// <summary>The total size of the domain's documents in bytes.</summary>
    public long TotalDocumentSize { get; init; }
}

/// <summary>A document that was stored.</summary>
public sealed record UploadedDocument
{
    /// <summary>The file name.</summary>
    public string? Name { get; init; }

    /// <summary>The size in bytes.</summary>
    public long Size { get; init; }

    /// <summary>The detected MIME type.</summary>
    public string? MimeType { get; init; }

    /// <summary>What the document is.</summary>
    public DocumentType Type { get; init; }

    /// <summary>When it was uploaded.</summary>
    public DateTime UploadedDate { get; init; }
}
