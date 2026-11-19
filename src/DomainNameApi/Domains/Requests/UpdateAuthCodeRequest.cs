namespace DomainNameApi.Domains;

/// <summary><c>PUT domains/auth</c>: sets a new transfer (EPP) code and returns it.</summary>
public sealed class UpdateAuthCodeRequest : BaseRequest<DomainAuthCode>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public UpdateAuthCodeRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Put;

    /// <inheritdoc />
    public override string Path => "domains/auth";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
