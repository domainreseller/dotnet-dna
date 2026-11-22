namespace DomainNameApi.Ssl;

/// <summary>Checks certificate IDs before anything is sent.</summary>
internal static class SslValidation
{
    /// <summary>
    /// Throws an <see cref="ArgumentException"/> for an empty certificate ID. That is what code gets when it never
    /// stored the ID, for example code written when certificates were still addressed by domain name.
    /// </summary>
    public static void RequireCertificateId(Guid certificateId)
    {
        if (certificateId == Guid.Empty)
        {
            throw new ArgumentException(
                "CertificateId must be a certificate ID (GUID) and cannot be empty. Certificates are no longer addressed by " +
                "domain name: use the CertificateId returned by Ssl.PurchaseAsync or Ssl.RenewAsync, or look it up with " +
                "Ssl.ListAsync(new ListCertificatesRequest { DomainName = \"example.com\" }).",
                "CertificateId");
        }
    }
}
