namespace DomainNameApi.Domains;

/// <summary><c>POST domains/transfers/query</c>: the state of a pending transfer.</summary>
public sealed class GetTransferStatusRequest : BaseRequest<TransferStatus>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public GetTransferStatusRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/transfers/query";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
