using System.Text.Json.Serialization;

namespace DomainNameApi.Ssl;

/// <summary>
/// <c>PUT ssls/configure</c>: submits the CSR, the validation method and the contact so the certificate can be issued.
/// A certificate must be configured within 15 days of its purchase or renewal; after that it is cancelled.
/// </summary>
public sealed class ConfigureCertificateRequest : BaseRequest<SslOperationResult>
{
    /// <summary>Creates the request.</summary>
    /// <param name="certificateId">
    /// The certificate to configure: the <see cref="SslOperationResult.CertificateId"/> returned by the purchase or the renewal.
    /// </param>
    /// <param name="csr">The CSR in PEM format. Its common name must be the certificate's domain.</param>
    /// <param name="contact">The certificate contact.</param>
    public ConfigureCertificateRequest(Guid certificateId, string csr, CertificateContact contact)
    {
        CertificateId = certificateId;
        Csr = csr;
        Contact = contact;
    }

    /// <summary>The certificate to configure.</summary>
    public Guid CertificateId { get; set; }

    /// <summary>The CSR in PEM format. Its common name must be the certificate's domain.</summary>
    public string Csr { get; set; }

    /// <summary>The private key that belongs to the CSR, if you want the API to keep it with the order.</summary>
    public string? PrivateKey { get; set; }

    /// <summary>How domain control is validated.</summary>
    [JsonPropertyName("authenticationType")]
    public ValidationMethod? ValidationMethod { get; set; }

    /// <summary>
    /// The approver address for e-mail validation. See <see cref="SslClient.GetApproverEmailsAsync(Guid, CancellationToken)"/>.
    /// </summary>
    [JsonPropertyName("approverMail")]
    public string? ApproverEmail { get; set; }

    /// <summary>The web server software.</summary>
    public WebServerType? WebServerType { get; set; }

    /// <summary>Additional domain names for multi-domain certificates.</summary>
    public string? SanList { get; set; }

    /// <summary>Wildcard names.</summary>
    public string? WildcardList { get; set; }

    /// <summary>The certificate contact.</summary>
    public CertificateContact Contact { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Put;

    /// <inheritdoc />
    public override string Path => "ssls/configure";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Write;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        SslValidation.RequireCertificateId(CertificateId);
        Require(Csr, nameof(Csr));
        Require(Contact, nameof(Contact));
        if (ValidationMethod == Ssl.ValidationMethod.Email && string.IsNullOrWhiteSpace(ApproverEmail))
        {
            throw new ArgumentException("E-mail validation needs an ApproverEmail.", nameof(ApproverEmail));
        }
    }
}
