namespace DomainNameApi.Orders;

/// <summary>Orders. Reach it through <see cref="DomainNameApiClient.Orders"/>.</summary>
public sealed class OrdersClient
{
    private readonly DomainNameApiClient _api;

    internal OrdersClient(DomainNameApiClient api) => _api = api;

    /// <summary>Returns an order and its lines.</summary>
    /// <param name="orderCode">The order code.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<Order> GetAsync(string orderCode, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetOrderRequest(orderCode), cancellationToken);
}
