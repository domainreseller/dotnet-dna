namespace DomainNameApi.Accounts;

/// <summary>Your reseller account: balances and the transaction ledger. Reach it through <see cref="DomainNameApiClient.Account"/>.</summary>
public sealed class AccountClient
{
    private readonly DomainNameApiClient _api;

    internal AccountClient(DomainNameApiClient api) => _api = api;

    /// <summary>Returns your reseller ID, name, price group and TRY/USD balances.</summary>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<ResellerAccount> GetAsync(CancellationToken cancellationToken = default) =>
        _api.SendAsync(new GetAccountRequest(), cancellationToken);

    /// <summary>Returns one page of the balance ledger.</summary>
    /// <param name="request">Filters and paging; null for the first page without filters.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public Task<PagedResult<AccountTransaction>> ListTransactionsAsync(ListTransactionsRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.SendAsync(request ?? new ListTransactionsRequest(), cancellationToken);

    /// <summary>Yields every transaction that matches the filters, page by page.</summary>
    /// <param name="request">Filters; null for all transactions.</param>
    /// <param name="cancellationToken">Stops the enumeration.</param>
    public IAsyncEnumerable<AccountTransaction> ListAllTransactionsAsync(ListTransactionsRequest? request = null, CancellationToken cancellationToken = default) =>
        _api.ListAllAsync(request ?? new ListTransactionsRequest(), cancellationToken);
}
