namespace DomainNameApi.Domains;

/// <summary><c>POST domains/unlock</c>: turns the transfer lock off, which is needed before transferring out.</summary>
public sealed class UnlockDomainRequest : BaseRequest
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public UnlockDomainRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "domains/unlock";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
