namespace DomainNameApi.Products;

/// <summary>The price of a product for an order type and period.</summary>
public sealed record ProductPrice
{
    /// <summary>The parent product, if any.</summary>
    public Guid? ParentProductId { get; init; }

    /// <summary>The product name.</summary>
    public string? ProductName { get; init; }

    /// <summary>The product code.</summary>
    public string? ProductCode { get; init; }

    /// <summary>The ID of the entity the product sells.</summary>
    public string? EntityId { get; init; }

    /// <summary>The kind of product.</summary>
    public ProductType ProductType { get; init; }

    /// <summary>Your price.</summary>
    public decimal SellingPrice { get; init; }

    /// <summary>The currency of <see cref="SellingPrice"/>.</summary>
    public string? SellingCurrency { get; init; }

    /// <summary>Products included in this one.</summary>
    public IReadOnlyList<Subproduct> Subproducts { get; init; } = [];
}

/// <summary>A product included in another one.</summary>
public sealed record Subproduct
{
    /// <summary>The parent product.</summary>
    public Guid ParentProductId { get; init; }

    /// <summary>The product name.</summary>
    public string? ProductName { get; init; }

    /// <summary>The product code.</summary>
    public string? ProductCode { get; init; }

    /// <summary>The kind of product.</summary>
    public ProductType ProductType { get; init; }

    /// <summary>The ID of the entity the product sells.</summary>
    public string? EntityId { get; init; }

    /// <summary>The cost price.</summary>
    public decimal BuyingPrice { get; init; }

    /// <summary>The currency of <see cref="BuyingPrice"/>.</summary>
    public string? BuyingCurrency { get; init; }

    /// <summary>Your price.</summary>
    public decimal SellingPrice { get; init; }

    /// <summary>The currency of <see cref="SellingPrice"/>.</summary>
    public string? SellingCurrency { get; init; }
}
