namespace DomainNameApi.Products;

/// <summary><c>GET products</c>: pages through the product catalogue.</summary>
public sealed class ListProductsRequest : PagedRequest<Product>
{
    /// <summary>Free-text search.</summary>
    public string? FilterText { get; set; }

    /// <summary>Only products of this kind.</summary>
    public ProductType? ProductType { get; set; }

    /// <inheritdoc />
    public override string Path => "products";

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query)
    {
        query.Add("FilterText", FilterText).Add("ProductType", ProductType);
        base.AddQueryParameters(query);
    }
}
