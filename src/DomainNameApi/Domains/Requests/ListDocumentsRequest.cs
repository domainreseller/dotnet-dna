namespace DomainNameApi.Domains;

/// <summary><c>GET domains/documents</c>: pages through the registry documents of a domain (mostly .tr).</summary>
public sealed class ListDocumentsRequest : PagedRequest<DomainDocument>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public ListDocumentsRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>Only these document types.</summary>
    public List<DocumentType> DocumentTypes { get; set; } = [];

    /// <inheritdoc />
    public override string Path => "domains/documents";

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query)
    {
        query.Add("DomainName", DomainName);
        if (DocumentTypes.Count > 0)
        {
            query.AddCommaSeparated("DocumentTypes", DocumentTypes);
        }
        base.AddQueryParameters(query);
    }

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        base.Validate();
    }
}
