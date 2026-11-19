namespace DomainNameApi.Orders;

/// <summary><c>GET order/{orderCode}</c>: an order and its lines.</summary>
public sealed class GetOrderRequest : BaseRequest<Order>
{
    /// <summary>Creates the request.</summary>
    /// <param name="orderCode">The order code.</param>
    public GetOrderRequest(string orderCode) => OrderCode = orderCode;

    /// <summary>The order code.</summary>
    public string OrderCode { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "order/" + Uri.EscapeDataString(OrderCode ?? "");

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void Validate() => Require(OrderCode, nameof(OrderCode));
}
