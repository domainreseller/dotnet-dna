using DomainNameApi.Ssl;

namespace DomainNameApi.Tests.Features;

public class SslClientTests
{
    private const string Operation = """{"success":true,"certificateId":"7d9f1a52-3c1e-4c9e-9a51-6b8c2f3e4d10","status":"Pending","dcvDetails":"_cname.example.com"}""";

    private static CertificateContact Contact() => new()
    {
        FirstName = "Ada",
        LastName = "Lovelace",
        CompanyName = "Example Ltd",
        Email = "ada@example.com",
        Address = "1 Example Street",
        PhoneCountryCode = "90",
        Phone = "5551234567",
        Country = "TR",
        City = "Istanbul",
        PostalCode = "34000",
    };

    [Fact]
    public async Task PurchaseIsBillable()
    {
        using var t = new TestClient(o => o.ReadOnly = true);

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Ssl.PurchaseAsync("example.com", "PKG-DV"));

        Assert.Equal(OperationSafety.Billable, e.Safety);
    }

    [Fact]
    public async Task PurchaseAndConfigure()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Operation).Respond(200, Operation);

        var bought = await t.Client.Ssl.PurchaseAsync("example.com", "PKG-DV");
        Assert.Equal("ssls/initialize", t.LastRequest.ApiPath);
        Assert.Equal("PKG-DV", t.LastRequest.Json.Str("packageCode"));
        Assert.Equal("Pending", bought.Status);

        await t.Client.Ssl.ConfigureAsync(new ConfigureCertificateRequest("example.com", "-----BEGIN CERTIFICATE REQUEST-----", Contact())
        {
            ValidationMethod = ValidationMethod.Email,
            ApproverEmail = "admin@example.com",
            WebServerType = WebServerType.Apache,
        });
        Assert.Equal("PUT", t.LastRequest.Method);
        Assert.Equal("ssls/configure", t.LastRequest.ApiPath);
        var body = t.LastRequest.Json;
        Assert.Equal("Email", body.Str("authenticationType"));
        Assert.Equal("admin@example.com", body.Str("approverMail"));
        Assert.Equal("Apache", body.Str("webServerType"));
        Assert.Equal("ada@example.com", body.GetProperty("contact").Str("eMail"));
        Assert.False(body.Has("privateKey"));
    }

    [Fact]
    public async Task EmailValidationNeedsAnApprover()
    {
        using var t = new TestClient();

        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Ssl.ConfigureAsync(
            new ConfigureCertificateRequest("example.com", "csr", Contact()) { ValidationMethod = "email" }));
    }

    [Fact]
    public async Task CsrStatusContentApproversReissueAndRevoke()
    {
        using var t = new TestClient();
        t.Handler
            .Respond(200, """{"csr":"-----CSR-----","privateKey":"-----KEY-----"}""")
            .Respond(200, """{"domainName":"example.com","isWildcard":false}""")
            .Respond(200, """{"domainName":"example.com","status":"Issued","isIssued":true,"certificateExpiresAt":"2027-11-19T00:00:00Z"}""")
            .Respond(200, """{"certificate":"CERT","intermediateCertificate1":"CA1","rootCertificate":"ROOT"}""")
            .Respond(200, """{"mails":["admin@example.com","webmaster@example.com"]}""")
            .Respond(200, Operation)
            .Respond(200, Operation);

        var csr = await t.Client.Ssl.GenerateCsrAsync(new GenerateCsrRequest("example.com")
        {
            Country = "TR", State = "Istanbul", Locality = "Istanbul", Organization = "Example", OrganizationUnit = "IT",
        });
        Assert.Equal("ssls/generate-csr", t.LastRequest.ApiPath);
        Assert.Equal("-----KEY-----", csr.PrivateKey);

        await t.Client.Ssl.DecodeCsrAsync(csr.Csr!);
        Assert.Equal("-----CSR-----", t.LastRequest.Json.Str("csr"));

        var status = await t.Client.Ssl.RefreshStatusAsync("example.com");
        Assert.Equal("PUT", t.LastRequest.Method);
        Assert.True(status.IsIssued);

        var content = await t.Client.Ssl.GetContentAsync("example.com");
        Assert.Equal("example.com", t.LastRequest.Query("DomainName"));
        Assert.Equal("CERT\nCA1\n", content.FullChain);

        var approvers = await t.Client.Ssl.GetApproverEmailsAsync("example.com", "PKG-DV");
        Assert.Equal("PKG-DV", t.LastRequest.Query("PackageCode"));
        Assert.Equal(2, approvers.Count);

        await t.Client.Ssl.ReissueAsync(new ReissueCertificateRequest("example.com") { ApproverEmail = "admin@example.com" });
        Assert.Equal("ssls/reissue", t.LastRequest.ApiPath);
        Assert.Equal("admin@example.com", t.LastRequest.Json.Str("approverMail"));

        await t.Client.Ssl.RevokeAsync("example.com");
        Assert.Equal("DELETE", t.LastRequest.Method);
        Assert.Equal("ssls/revoke", t.LastRequest.ApiPath);
    }

    [Fact]
    public async Task GenerateCsrNeedsTheFullSubject()
    {
        using var t = new TestClient();

        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Ssl.GenerateCsrAsync(new GenerateCsrRequest("example.com")));
    }
}
