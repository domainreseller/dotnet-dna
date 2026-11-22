namespace DomainNameApi.Ssl;

/// <summary>An issued certificate and its chain, in PEM format.</summary>
public sealed record CertificateContent
{
    /// <summary>The ID of the certificate this content belongs to.</summary>
    public Guid CertificateId { get; init; }

    /// <summary>The certificate.</summary>
    public string? Certificate { get; init; }

    /// <summary>The first intermediate certificate.</summary>
    public string? IntermediateCertificate1 { get; init; }

    /// <summary>The second intermediate certificate, if any.</summary>
    public string? IntermediateCertificate2 { get; init; }

    /// <summary>The root certificate.</summary>
    public string? RootCertificate { get; init; }

    /// <summary>The certificate followed by its intermediates: what most web servers expect as the "full chain" file.</summary>
    public string FullChain =>
        string.Join("\n", new[] { Certificate, IntermediateCertificate1, IntermediateCertificate2 }
            .Where(pem => !string.IsNullOrWhiteSpace(pem))
            .Select(pem => pem!.Trim())) + "\n";
}
