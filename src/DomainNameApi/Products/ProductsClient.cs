namespace DomainNameApi.Products;

/// <summary>The product catalogue and prices. Reach it through <see cref="DomainNameApiClient.Products"/>.</summary>
public sealed class ProductsClient
{
    private readonly DomainNameApiClient _api;

    internal ProductsClient(DomainNameApiClient api) => _api = api;

    /// <summary>Returns one page of the product catalogue.</summary>
    /// <param name="request">Filters and paging; null for the first page.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<PagedResult<Product>> ListAsync(ListProductsRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request ?? new ListProductsRequest(), cancellationToken);

    /// <summary>Yields every product that matches the filters.</summary>
    /// <param name="request">Filters; null for all products.</param>
    /// <param name="cancellationToken">Stops the enumeration.</param>
    public IAsyncEnumerable<Product> ListAllAsync(ListProductsRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.ListAllAsync(request ?? new ListProductsRequest(), cancellationToken);

    /// <summary>Returns the price of a product for an order type and period.</summary>
    /// <param name="productName">The product name, for example a TLD such as <c>com</c>.</param>
    /// <param name="orderType">Register, Renew, Transfer and so on.</param>
    /// <param name="years">The period in years.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<ProductPrice> GetPriceAsync(string productName, OrderType orderType, int years = 1, CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetProductPriceRequest(productName, orderType, years), cancellationToken);

    /// <summary>Returns one page of TLDs with prices, periods, rules and attributes.</summary>
    /// <param name="request">Filters and paging; null for the first page.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<PagedResult<Tld>> ListTldsAsync(ListTldsRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request ?? new ListTldsRequest(), cancellationToken);

    /// <summary>Yields every TLD that matches the filters.</summary>
    /// <param name="request">Filters; null for all TLDs.</param>
    /// <param name="cancellationToken">Stops the enumeration.</param>
    public IAsyncEnumerable<Tld> ListAllTldsAsync(ListTldsRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.ListAllAsync(request ?? new ListTldsRequest(), cancellationToken);

    /// <summary>Returns one TLD by name, or null when it is not sold.</summary>
    /// <param name="name">The TLD, with or without a leading dot: <c>com</c>, <c>.com.tr</c>.</param>
    /// <param name="currency">The price currency; null for the account default.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public async Task<Tld?> GetTldAsync(string name, string? currency = null, CancellationToken cancellationToken = default)
    {
        var wanted = (name ?? throw new ArgumentNullException(nameof(name))).Trim().TrimStart('.');
        var request = new ListTldsRequest { FilterText = wanted, Currency = currency };
        await foreach (var tld in _api.ListAllAsync(request, cancellationToken).ConfigureAwait(false))
        {
            if (string.Equals(tld.Name?.TrimStart('.'), wanted, StringComparison.OrdinalIgnoreCase))
            {
                return tld;
            }
        }
        return null;
    }

    /// <summary>Returns one page of SSL certificate products with prices.</summary>
    /// <param name="request">Filters and paging; null for the first page.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<PagedResult<SslProduct>> ListSslProductsAsync(ListSslProductsRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request ?? new ListSslProductsRequest(), cancellationToken);

    /// <summary>Yields every SSL certificate product that matches the filters.</summary>
    /// <param name="request">Filters; null for all products.</param>
    /// <param name="cancellationToken">Stops the enumeration.</param>
    public IAsyncEnumerable<SslProduct> ListAllSslProductsAsync(ListSslProductsRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.ListAllAsync(request ?? new ListSslProductsRequest(), cancellationToken);
}
