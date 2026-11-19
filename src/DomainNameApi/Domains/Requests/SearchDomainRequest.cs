namespace DomainNameApi.Domains;

/// <summary>
/// <c>POST domains/search</c>: whether a domain can be registered, its price and whether the registry
/// needs documents. The gateway allows 2 searches per second.
/// </summary>
public sealed class SearchDomainRequest : BaseRequest<DomainSearchResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain, for example <c>example.com</c>.</param>
    /// <param name="currency">The price currency, for example <c>USD</c> or <c>TRY</c>; null for the account default.</param>
    public SearchDomainRequest(string domainName, string? currency = null)
    {
        DomainName = domainName;
        Currency = currency;
    }

    /// <summary>The domain name.</summary>
    public string DomainName { get; set; }

    /// <summary>The price currency.</summary>
    public string? Currency { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/search";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
