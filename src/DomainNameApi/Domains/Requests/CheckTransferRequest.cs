namespace DomainNameApi.Domains;

/// <summary><c>POST domains/transfers/check</c>: whether a domain can be transferred in with an auth code. Charges nothing.</summary>
public sealed class CheckTransferRequest : BaseRequest<TransferCheck>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="authCode">The transfer (EPP) code.</param>
    public CheckTransferRequest(string domainName, string authCode)
    {
        DomainName = domainName;
        AuthCode = authCode;
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The transfer (EPP) code.</summary>
    public string AuthCode { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/transfers/check";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(AuthCode, nameof(AuthCode));
    }
}
