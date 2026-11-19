using System.Text.Json.Serialization;

namespace DomainNameApi.Ssl;

/// <summary><c>POST ssls/reissue</c>: reissues a certificate with a new CSR and key, or a new approver.</summary>
public sealed class ReissueCertificateRequest : BaseRequest<SslOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The domain.</param>
    public ReissueCertificateRequest(string domainName) => DomainName = domainName;

    /// <summary>The domain.</summary>
    public string DomainName { get; set; }

    /// <summary>A new CSR, or null to keep the current one.</summary>
    public string? Csr { get; set; }

    /// <summary>The private key of the new CSR.</summary>
    public string? PrivateKey { get; set; }

    /// <summary>A new approver address for e-mail validation.</summary>
    [JsonPropertyName("approverMail")]
    public string? ApproverEmail { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "ssls/reissue";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate() => Require(DomainName, nameof(DomainName));
}
