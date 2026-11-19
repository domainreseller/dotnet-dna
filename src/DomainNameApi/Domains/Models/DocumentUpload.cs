namespace DomainNameApi.Domains;

/// <summary>A document to upload for a domain. Create one with <see cref="FromFile"/> or <see cref="FromBytes"/>.</summary>
public sealed record DocumentUpload
{
    /// <summary>What the document is.</summary>
    public DocumentType DocumentType { get; init; }

    /// <summary>The file name, including its extension (for example <c>id.pdf</c>).</summary>
    public string? FileName { get; init; }

    /// <summary>The file content, Base64-encoded.</summary>
    public string? FileContent { get; init; }

    /// <summary>Creates an upload from bytes.</summary>
    /// <param name="documentType">What the document is.</param>
    /// <param name="fileName">The file name, including its extension.</param>
    /// <param name="content">The file content.</param>
    public static DocumentUpload FromBytes(DocumentType documentType, string fileName, byte[] content) => new()
    {
        DocumentType = documentType,
        FileName = fileName,
        FileContent = Convert.ToBase64String(content ?? throw new ArgumentNullException(nameof(content))),
    };

    /// <summary>Creates an upload from a file on disk.</summary>
    /// <param name="documentType">What the document is.</param>
    /// <param name="path">The file path; its name is used as the file name.</param>
    public static DocumentUpload FromFile(DocumentType documentType, string path) =>
        FromBytes(documentType, System.IO.Path.GetFileName(path), File.ReadAllBytes(path));
}
