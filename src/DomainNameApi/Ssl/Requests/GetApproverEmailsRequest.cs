namespace DomainNameApi.Ssl;

/// <summary><c>GET ssls/approvers</c>: the e-mail addresses allowed for e-mail validation of a domain.</summary>
public sealed class GetApproverEmailsRequest : BaseRequest<ApproverEmailsResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    /// <param name="packageCode">The product code, when you have not bought the certificate yet.</param>
    public GetApproverEmailsRequest(string domainName, string? packageCode = null)
    {
        DomainName = domainName;
        PackageCode = packageCode;
    }

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>The product code.</summary>
    public string? PackageCode { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Get;

    /// <inheritdoc />
    public override string Path => "ssls/approvers";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void AddQueryParameters(QueryParameters query) =>
        query.Add("DomainName", DomainName).Add("PackageCode", PackageCode);

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
