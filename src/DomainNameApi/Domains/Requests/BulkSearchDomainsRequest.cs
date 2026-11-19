namespace DomainNameApi.Domains;

/// <summary><c>POST domains/bulk-search</c>: availability and prices of several domains in one call.</summary>
public sealed class BulkSearchDomainsRequest : BaseRequest<BulkSearchResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainNames">The domains, for example <c>example.com</c> and <c>example.net</c>.</param>
    /// <param name="currency">The price currency; null for the account default.</param>
    public BulkSearchDomainsRequest(IEnumerable<string> domainNames, string? currency = null)
    {
        DomainNames = domainNames?.ToList() ?? [];
        Currency = currency;
    }

    /// <summary>The domain names.</summary>
    public List<string> DomainNames { get; set; }

    /// <summary>The price currency, used for every domain.</summary>
    public string? Currency { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/bulk-search";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <summary>The API expects an array of <c>{domainName, currency}</c> objects.</summary>
    protected internal override object GetBody() =>
        DomainNames.Select(name => new SearchItem { DomainName = name.Trim(), Currency = Currency }).ToList();

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainNames, nameof(DomainNames));
        if (DomainNames.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("DomainNames contains an empty value.", nameof(DomainNames));
        }
    }

    private sealed class SearchItem
    {
        public string? DomainName { get; set; }

        public string? Currency { get; set; }
    }
}
