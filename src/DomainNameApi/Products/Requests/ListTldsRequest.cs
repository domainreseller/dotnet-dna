namespace DomainNameApi.Products;

/// <summary><c>GET products/tlds</c>: pages through the TLDs with prices, periods, rules and attributes. Not rate limited.</summary>
public sealed class ListTldsRequest : PagedRequest<Tld>
{
    /// <summary>Free-text search, for example <c>com</c>.</summary>
    public string? FilterText { get; set; }

    /// <summary>The price currency, for example <c>USD</c> or <c>TRY</c>.</summary>
    public string? Currency { get; set; }

    /// <inheritdoc />
    public override string Path => "products/tlds";

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query)
    {
        query.Add("FilterText", FilterText).Add("Currency", Currency);
        base.AddQueryParameters(query);
    }
}
