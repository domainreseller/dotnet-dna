namespace DomainNameApi.Accounts;

/// <summary>
/// <c>GET resellers/me</c>: your reseller profile (name, company, price group, account currency, status and flags).
/// For the balances use <see cref="GetAccountRequest"/>.
/// </summary>
public sealed class GetProfileRequest : BaseRequest<ResellerProfile>
{
    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "resellers/me";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;
}
