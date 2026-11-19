namespace DomainNameApi.Domains;

/// <summary><c>POST domains/documents</c>: uploads registry documents for a domain.</summary>
public sealed class UploadDocumentsRequest : BaseRequest<DocumentUploadResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="documents">The documents. Create them with <see cref="DocumentUpload.FromFile"/>.</param>
    public UploadDocumentsRequest(string domainName, IEnumerable<DocumentUpload> documents)
    {
        DomainName = domainName;
        Documents = documents?.ToList() ?? [];
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The documents.</summary>
    public List<DocumentUpload> Documents { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/documents";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(Documents, nameof(Documents));
        if (Documents.Any(d => d is null || string.IsNullOrWhiteSpace(d.FileName) || string.IsNullOrEmpty(d.FileContent)))
        {
            throw new ArgumentException("Every document needs a file name and content.", nameof(Documents));
        }
    }
}
