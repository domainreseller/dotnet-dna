namespace DomainNameApi.Products;

/// <summary>A product in the catalogue.</summary>
public sealed record Product
{
    /// <summary>The product ID.</summary>
    public Guid Id { get; init; }

    /// <summary>The parent product, if any.</summary>
    public Guid? ParentProductId { get; init; }

    /// <summary>The product name, for example a TLD such as <c>com</c>.</summary>
    public string? ProductName { get; init; }

    /// <summary>The product code.</summary>
    public string? ProductCode { get; init; }

    /// <summary>The ID of the entity the product sells.</summary>
    public string? EntityId { get; init; }

    /// <summary>The kind of product.</summary>
    public ProductType ProductType { get; init; }
}
