namespace DomainNameApi.Hosting;

/// <summary><c>GET hostings/by-token</c>: a hosting, looked up by the token returned when it was ordered.</summary>
public sealed class GetHostingByTokenRequest : BaseRequest<HostingService>
{
    /// <summary>Creates the request.</summary>
    /// <param name="token">The token from <see cref="HostingCreateResult.Token"/>.</param>
    public GetHostingByTokenRequest(string token) => Token = token;

    /// <summary>The token.</summary>
    public string Token { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "hostings/by-token";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) => query.Add("token", Token);

    /// <inheritdoc />
    protected internal override void Validate() => Require(Token, nameof(Token));
}
