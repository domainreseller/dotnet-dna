namespace DomainNameApi.Ssl;

/// <summary>
/// <c>POST ssls/generate-csr</c>: generates a certificate signing request and a private key.
/// Charges nothing. Keep the private key: you need it to install the certificate.
/// </summary>
public sealed class GenerateCsrRequest : BaseRequest<GeneratedCsr>
{
    /// <summary>Creates the request.</summary>
    /// <param name="domainName">The common name, for example <c>www.example.com</c> or <c>*.example.com</c>.</param>
    public GenerateCsrRequest(string domainName) => DomainName = domainName;

    /// <summary>The common name.</summary>
    public string DomainName { get; set; }

    /// <summary>Two-letter country code.</summary>
    public string? Country { get; set; }

    /// <summary>State or province.</summary>
    public string? State { get; set; }

    /// <summary>City.</summary>
    public string? Locality { get; set; }

    /// <summary>Organisation.</summary>
    public string? Organization { get; set; }

    /// <summary>Organisational unit.</summary>
    public string? OrganizationUnit { get; set; }

    /// <summary>Additional domain names for multi-domain certificates.</summary>
    public List<string>? AlternativeNames { get; set; }

    /// <inheritdoc />
    public override HttpMethod Method => HttpMethod.Post;

    /// <inheritdoc />
    public override string Path => "ssls/generate-csr";

    /// <inheritdoc />
    public override OperationSafety Safety => OperationSafety.Read;

    /// <inheritdoc />
    protected internal override void Validate()
    {
        Require(DomainName, nameof(DomainName));
        Require(Country, nameof(Country));
        Require(State, nameof(State));
        Require(Locality, nameof(Locality));
        Require(Organization, nameof(Organization));
        Require(OrganizationUnit, nameof(OrganizationUnit));
    }
}
