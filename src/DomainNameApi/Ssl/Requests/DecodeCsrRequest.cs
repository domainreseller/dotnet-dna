namespace DomainNameApi.Ssl;

/// <summary><c>POST ssls/check-csr</c>: decodes a CSR and returns its subject.</summary>
public sealed class DecodeCsrRequest : BaseRequest<CsrDetails>
{
    /// <summary>Creates the request.</summary>
    /// <param name="csr">The CSR in PEM format.</param>
    public DecodeCsrRequest(string csr) => Csr = csr;

    /// <summary>The CSR in PEM format.</summary>
    public string Csr { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "ssls/check-csr";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void Validate() => Require(Csr, nameof(Csr));
}
