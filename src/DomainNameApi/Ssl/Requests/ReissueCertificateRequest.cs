using System.Text.Json.Serialization;

namespace DomainNameApi.Ssl;

/// <summary><c>POST ssls/reissue</c>: reissues a certificate with a new CSR and key, or a new approver.</summary>
public sealed class ReissueCertificateRequest : BaseRequest<SslOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="certificateId">The certificate to reissue.</param>
    public ReissueCertificateRequest(Guid certificateId) => CertificateId = certificateId;

    /// <summary>The certificate to reissue.</summary>
    public Guid CertificateId { get; set; }

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
    protected internal override void Validate() => SslValidation.RequireCertificateId(CertificateId);
}
