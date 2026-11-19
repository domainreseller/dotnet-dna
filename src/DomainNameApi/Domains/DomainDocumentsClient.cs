namespace DomainNameApi.Domains;

/// <summary>Registry documents of domains (mostly .tr). Reach it through <see cref="DomainsClient.Documents"/>.</summary>
public sealed class DomainDocumentsClient
{
    private readonly DomainNameApiClient _api;

    internal DomainDocumentsClient(DomainNameApiClient api) => _api = api;

    /// <summary>Returns one page of the documents of a domain.</summary>
    /// <param name="request">The domain, filters and paging.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<PagedResult<DomainDocument>> ListAsync(ListDocumentsRequest request, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request, cancellationToken);

    /// <summary>Yields every document of a domain.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="cancellationToken">Stops the enumeration.</param>
    public IAsyncEnumerable<DomainDocument> ListAllAsync(string domainName, CancellationToken cancellationToken = default) =>
        _api.ListAllAsync(new ListDocumentsRequest(domainName), cancellationToken);

    /// <summary>Uploads documents for a domain.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="documents">The documents. Create them with <see cref="DocumentUpload.FromFile"/>.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<DocumentUploadResult> UploadAsync(string domainName, IEnumerable<DocumentUpload> documents, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new UploadDocumentsRequest(domainName, documents), cancellationToken);
}
