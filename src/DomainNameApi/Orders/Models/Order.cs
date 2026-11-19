using System.Text.Json.Serialization;

namespace DomainNameApi.Orders;

/// <summary>An order and its lines.</summary>
public sealed record Order
{
    /// <summary>The order code.</summary>
    public string? OrderCode { get; init; }

    /// <summary>The state.</summary>
    public OrderStatus Status { get; init; }

    /// <summary>The total amount.</summary>
    public decimal Amount { get; init; }

    /// <summary>The currency of <see cref="Amount"/>.</summary>
    public string? AmountCurrency { get; init; }

    /// <summary>The order lines.</summary>
    [JsonPropertyName("products")]
    public IReadOnlyList<OrderLine> Lines { get; init; } = [];
}

/// <summary>One line of an order.</summary>
public sealed record OrderLine
{
    /// <summary>The product code.</summary>
    public string? ProductCode { get; init; }

    /// <summary>The product name.</summary>
    public string? Name { get; init; }

    /// <summary>A description, usually naming the domain.</summary>
    public string? Description { get; init; }

    /// <summary>The kind of product.</summary>
    public ProductType ProductType { get; init; }

    /// <summary>The kind of order.</summary>
    public OrderType OrderType { get; init; }

    /// <summary>The state of the line.</summary>
    public OrderLineStatus Status { get; init; }

    /// <summary>The price.</summary>
    public decimal Price { get; init; }

    /// <summary>The currency of <see cref="Price"/>.</summary>
    public string? PriceCurrency { get; init; }

    /// <summary>The period in years.</summary>
    public int Period { get; init; }

    /// <summary>Extra data the API stores with the line.</summary>
    public string? AdditionalData { get; init; }
}
