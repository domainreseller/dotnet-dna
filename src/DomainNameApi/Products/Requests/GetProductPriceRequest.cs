namespace DomainNameApi.Products;

/// <summary><c>GET products/info</c>: the price of a product for an order type and period.</summary>
public sealed class GetProductPriceRequest : BaseRequest<ProductPrice>
{
    /// <summary>Creates the request.</summary>
    /// <param name="productName">The product name, for example a TLD such as <c>com</c>.</param>
    /// <param name="orderType">Register, Renew, Transfer and so on.</param>
    /// <param name="period">The period in years.</param>
    public GetProductPriceRequest(string productName, OrderType orderType, int period = 1)
    {
        ProductName = productName;
        OrderType = orderType;
        Period = period;
    }

    /// <summary>The product name.</summary>
    public string ProductName { get; set; }

    /// <summary>The kind of order.</summary>
    public OrderType OrderType { get; set; }

    /// <summary>The period in years.</summary>
    public int Period { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "products/info";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) =>
        query.Add("ProductName", ProductName).Add("OrderType", (OrderType?)OrderType).Add("Period", Period);

    /// <inheritdoc />
    protected internal override void Validate() => Require(ProductName, nameof(ProductName));
}
