namespace DomainNameApi.Products;

/// <summary><c>GET products/ssls</c>: pages through the SSL certificate products with prices.</summary>
public sealed class ListSslProductsRequest : PagedRequest<SslProduct>
{
    /// <summary>Free-text search.</summary>
    public string? FilterText { get; set; }

    /// <summary>The price currency.</summary>
    public string? Currency { get; set; }

    /// <summary>Only this validation level, as the API numbers it.</summary>
    public int? ValidationType { get; set; }

    /// <summary>Only wildcard (or only non-wildcard) products.</summary>
    public bool? IsWildcard { get; set; }

    /// <summary>Only multi-domain (or only single-domain) products.</summary>
    public bool? IsMultiDomain { get; set; }

    /// <summary>Only products of this certificate authority.</summary>
    public string? VendorName { get; set; }

    /// <inheritdoc />
    public override string Path => "products/ssls";

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query)
    {
        query.Add("FilterText", FilterText)
             .Add("Currency", Currency)
             .Add("ValidationType", ValidationType)
             .Add("IsWildcard", IsWildcard)
             .Add("IsMultiDomain", IsMultiDomain)
             .Add("VendorName", VendorName);
        base.AddQueryParameters(query);
    }
}
