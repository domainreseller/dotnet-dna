namespace DomainNameApi.Accounts;

/// <summary>
/// <c>GET deposit/accounts/me</c>: your reseller ID, name, price group and TRY/USD balances.
/// Also the cheapest way to check that the credentials work.
/// </summary>
public sealed class GetAccountRequest : BaseRequest<ResellerAccount>
{
    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "deposit/accounts/me";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;
}
