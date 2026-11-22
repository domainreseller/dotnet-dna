using DomainNameApi.Ssl;

namespace DomainNameApi.Tests.Features;

public class SslClientTests
{
    private const string Id = "7d9f1a52-3c1e-4c9e-9a51-6b8c2f3e4d10";
    private const string RenewalId = "0b6a5c1e-8f2d-4a7b-b3c9-5e1d2f3a4b6c";
    private const string Operation = """{"success":true,"certificateId":"7d9f1a52-3c1e-4c9e-9a51-6b8c2f3e4d10","status":"Pending","dcvDetails":"_cname.example.com"}""";

    private static readonly Guid CertificateId = Guid.Parse(Id);

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

    private static string ListPage(long total, params string[] domains) =>
        JsonSerializer.Serialize(new
        {
            totalCount = total,
            items = domains.Select(d => new { certificateId = Guid.NewGuid(), domainName = d, status = "Active" }),
        });

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
        Assert.Equal("example.com", t.LastRequest.Json.Str("domainName"));
        Assert.Equal("PKG-DV", t.LastRequest.Json.Str("packageCode"));
        Assert.Equal("Pending", bought.Status);
        Assert.Equal(CertificateId, bought.CertificateId);

        await t.Client.Ssl.ConfigureAsync(new ConfigureCertificateRequest(bought.CertificateId, "-----BEGIN CERTIFICATE REQUEST-----", Contact())
        {
            ValidationMethod = ValidationMethod.Email,
            ApproverEmail = "admin@example.com",
            WebServerType = WebServerType.Apache,
        });
        Assert.Equal("PUT", t.LastRequest.Method);
        Assert.Equal("ssls/configure", t.LastRequest.ApiPath);
        var body = t.LastRequest.Json;
        Assert.Equal(Id, body.Str("certificateId"));
        Assert.False(body.Has("domainName"));
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
            new ConfigureCertificateRequest(CertificateId, "csr", Contact()) { ValidationMethod = "email" }));
    }

    [Fact]
    public async Task CsrStatusDetailsContentReissueAndRevoke()
    {
        using var t = new TestClient();
        t.Handler
            .Respond(200, """{"csr":"-----CSR-----","privateKey":"-----KEY-----"}""")
            .Respond(200, """{"domainName":"example.com","isWildcard":false}""")
            .Respond(200, """{"certificateId":"7d9f1a52-3c1e-4c9e-9a51-6b8c2f3e4d10","domainName":"example.com","status":"Active","isIssued":true,"certificateExpiresAt":"2027-11-19T00:00:00Z"}""")
            .Respond(200, """{"certificateId":"7d9f1a52-3c1e-4c9e-9a51-6b8c2f3e4d10","domainName":"example.com","status":"Active","packageCode":"PKG-DV","subscription":{"purchasedAt":"2026-11-19T00:00:00Z","expiresAt":"2027-11-19T00:00:00Z"},"contacts":null}""")
            .Respond(200, """{"certificateId":"7d9f1a52-3c1e-4c9e-9a51-6b8c2f3e4d10","certificate":"CERT","intermediateCertificate1":"CA1","rootCertificate":"ROOT"}""")
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

        var status = await t.Client.Ssl.RefreshStatusAsync(CertificateId);
        Assert.Equal("PUT", t.LastRequest.Method);
        Assert.Equal("ssls/check-status", t.LastRequest.ApiPath);
        Assert.Equal(new[] { "certificateId" }, t.LastRequest.Json.Names());
        Assert.Equal(Id, t.LastRequest.Json.Str("certificateId"));
        Assert.True(status.IsIssued);
        Assert.Equal(CertificateId, status.CertificateId);

        var details = await t.Client.Ssl.GetAsync(CertificateId);
        Assert.Equal("GET", t.LastRequest.Method);
        Assert.Equal("ssls/certificate", t.LastRequest.ApiPath);
        Assert.Equal(Id, t.LastRequest.Query("CertificateId"));
        Assert.Null(t.LastRequest.Query("DomainName"));
        Assert.Equal(CertificateId, details.CertificateId);
        Assert.Equal(new DateTime(2027, 11, 19, 0, 0, 0, DateTimeKind.Utc), details.Subscription?.ExpiresAt);
        Assert.Empty(details.Contacts);

        var content = await t.Client.Ssl.GetContentAsync(CertificateId);
        Assert.Equal("ssls/certificate-content", t.LastRequest.ApiPath);
        Assert.Equal(Id, t.LastRequest.Query("CertificateId"));
        Assert.Null(t.LastRequest.Query("DomainName"));
        Assert.Equal(CertificateId, content.CertificateId);
        Assert.Equal("CERT\nCA1\n", content.FullChain);

        await t.Client.Ssl.ReissueAsync(new ReissueCertificateRequest(CertificateId) { ApproverEmail = "admin@example.com" });
        Assert.Equal("ssls/reissue", t.LastRequest.ApiPath);
        Assert.Equal(Id, t.LastRequest.Json.Str("certificateId"));
        Assert.False(t.LastRequest.Json.Has("domainName"));
        Assert.Equal("admin@example.com", t.LastRequest.Json.Str("approverMail"));

        await t.Client.Ssl.RevokeAsync(CertificateId);
        Assert.Equal("DELETE", t.LastRequest.Method);
        Assert.Equal("ssls/revoke", t.LastRequest.ApiPath);
        Assert.Equal(Id, t.LastRequest.Query("CertificateId"));
        Assert.Null(t.LastRequest.Body); // the API reads the query string, not a body
    }

    [Fact]
    public async Task RenewReturnsTheNewCertificate()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, $$"""{"success":true,"message":"SSL certificate renewal initialized successfully. Waiting for installation.","certificateId":"{{RenewalId}}","status":"WaitingForInstall"}""");

        var renewal = await t.Client.Ssl.RenewAsync(CertificateId);

        Assert.Equal("POST", t.LastRequest.Method);
        Assert.Equal("ssls/renew", t.LastRequest.ApiPath);
        Assert.Equal(new[] { "certificateId" }, t.LastRequest.Json.Names());
        Assert.Equal(Id, t.LastRequest.Json.Str("certificateId"));
        Assert.Equal(Guid.Parse(RenewalId), renewal.CertificateId);
        Assert.NotEqual(CertificateId, renewal.CertificateId);
        Assert.Equal("WaitingForInstall", renewal.Status);
    }

    [Fact]
    public async Task RenewIsBillable()
    {
        using var t = new TestClient(o => o.ReadOnly = true);

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Ssl.RenewAsync(CertificateId));

        Assert.Equal(OperationSafety.Billable, e.Safety);
        Assert.Equal(ApiErrorKind.ReadOnly, e.Kind);
        Assert.Empty(t.Handler.Requests);
    }

    [Fact]
    public async Task RenewIsNotSentAgainAfterANetworkError()
    {
        using var t = new TestClient();
        t.Handler.Fail(new HttpRequestException("connection reset"));

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Ssl.RenewAsync(CertificateId));

        Assert.True(e.OutcomeUnknown);
        Assert.Single(t.Handler.Requests); // a renewal charges the balance: never sent twice
    }

    [Fact]
    public async Task ListSendsTheFilterAndReadsTheItems()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, $$"""
            {"totalCount":2,"items":[
              {"certificateId":"{{RenewalId}}","domainName":"example.com","packageCode":"PKG-DV","packageName":"Positive SSL",
               "status":"WaitingForInstall","statusReason":"None","isConfigured":false,"isIssued":false,
               "purchasedAt":"2027-11-01T10:00:00Z","subscriptionExpiresAt":"2028-11-01T10:00:00Z",
               "certificateExpiresAt":null,"renewedFromCertificateId":"{{Id}}"},
              {"certificateId":"{{Id}}","domainName":"example.com","packageCode":"PKG-DV","packageName":"Positive SSL",
               "status":"Active","statusReason":"None","isConfigured":true,"isIssued":true,
               "purchasedAt":"2026-11-19T00:00:00Z","subscriptionExpiresAt":"2027-11-19T00:00:00Z",
               "certificateExpiresAt":"2027-11-20T23:59:59Z","renewedFromCertificateId":null}
            ]}
            """);

        var page = await t.Client.Ssl.ListAsync(new ListCertificatesRequest { DomainName = "example.com", SkipCount = 0, MaxResultCount = 50 });

        var request = t.LastRequest;
        Assert.Equal("GET", request.Method);
        Assert.Equal("ssls", request.ApiPath);
        Assert.Equal("example.com", request.Query("DomainName"));
        Assert.Equal("0", request.Query("SkipCount"));
        Assert.Equal("50", request.Query("MaxResultCount"));
        Assert.Equal(new[] { "DomainName", "SkipCount", "MaxResultCount" }, request.QueryParameters.Select(p => p.Key));

        Assert.Equal(2, page.TotalCount);
        var renewal = page.Items[0];
        Assert.Equal(Guid.Parse(RenewalId), renewal.CertificateId);
        Assert.Equal("WaitingForInstall", renewal.Status);
        Assert.Equal("None", renewal.StatusReason);
        Assert.False(renewal.IsConfigured);
        Assert.Null(renewal.CertificateExpiresAt);
        Assert.Equal(CertificateId, renewal.RenewedFromCertificateId);

        var current = page.Items[1];
        Assert.Equal(CertificateId, current.CertificateId);
        Assert.Equal("example.com", current.DomainName);
        Assert.Equal("PKG-DV", current.PackageCode);
        Assert.Equal("Positive SSL", current.PackageName);
        Assert.True(current.IsConfigured);
        Assert.True(current.IsIssued);
        Assert.Equal(new DateTime(2026, 11, 19, 0, 0, 0, DateTimeKind.Utc), current.PurchasedAt);
        Assert.Equal(new DateTime(2027, 11, 19, 0, 0, 0, DateTimeKind.Utc), current.SubscriptionExpiresAt);
        Assert.Equal(new DateTime(2027, 11, 20, 23, 59, 59, DateTimeKind.Utc), current.CertificateExpiresAt);
        Assert.Null(current.RenewedFromCertificateId);
    }

    [Fact]
    public async Task ListWithoutARequestSendsNoParameters()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, ListPage(0));

        var page = await t.Client.Ssl.ListAsync();

        Assert.Equal("ssls", t.LastRequest.ApiPath);
        Assert.Empty(t.LastRequest.QueryParameters);
        Assert.Empty(page.Items);
    }

    [Fact]
    public async Task ListAllPagesUntilTheTotalIsReached()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, ListPage(3, "a.example.com", "b.example.com")).Respond(200, ListPage(3, "c.example.com"));
        var request = new ListCertificatesRequest { MaxResultCount = 2 };

        var all = new List<CertificateListItem>();
        await foreach (var item in t.Client.Ssl.ListAllAsync(request))
        {
            all.Add(item);
        }

        Assert.Equal(new[] { "a.example.com", "b.example.com", "c.example.com" }, all.Select(i => i.DomainName));
        Assert.Equal("0", t.Handler.Requests[0].Query("SkipCount"));
        Assert.Equal("2", t.Handler.Requests[1].Query("SkipCount"));
        Assert.Equal("2", t.Handler.Requests[1].Query("MaxResultCount"));
        Assert.Null(request.SkipCount); // the caller's request is not changed
    }

    [Fact]
    public async Task ListCannotBeSorted()
    {
        using var t = new TestClient();

        var e = await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Ssl.ListAsync(new ListCertificatesRequest { Sorting = "purchasedAt desc" }));

        Assert.Equal("Sorting", e.ParamName);
        Assert.Empty(t.Handler.Requests);
    }

    [Fact]
    public async Task ApproversOfAnExistingCertificate()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"mails":["admin@example.com","webmaster@example.com"]}""");

        var approvers = await t.Client.Ssl.GetApproverEmailsAsync(CertificateId);

        Assert.Equal("GET", t.LastRequest.Method);
        Assert.Equal("ssls/approvers", t.LastRequest.ApiPath);
        Assert.Equal(Id, t.LastRequest.Query("CertificateId"));
        Assert.Null(t.LastRequest.Query("DomainName"));
        Assert.Null(t.LastRequest.Query("PackageCode"));
        Assert.Equal(new[] { "admin@example.com", "webmaster@example.com" }, approvers);
    }

    [Fact]
    public async Task ApproversBeforeAPurchase()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"mails":["admin@example.com"]}""");

        var approvers = await t.Client.Ssl.GetApproverEmailsAsync("example.com", "PKG-DV");

        Assert.Equal("ssls/approvers", t.LastRequest.ApiPath);
        Assert.Equal("example.com", t.LastRequest.Query("DomainName"));
        Assert.Equal("PKG-DV", t.LastRequest.Query("PackageCode"));
        Assert.Null(t.LastRequest.Query("CertificateId"));
        Assert.Single(approvers);
    }

    [Fact]
    public async Task ApproversNeedACertificateOrADomainWithItsPackage()
    {
        using var t = new TestClient();

        var noPackage = await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Ssl.GetApproverEmailsAsync("example.com", ""));
        Assert.Equal("PackageCode", noPackage.ParamName);

        var noDomain = await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Ssl.GetApproverEmailsAsync(" ", "PKG-DV"));
        Assert.Contains("CertificateId", noDomain.Message);
        Assert.Contains("DomainName and PackageCode", noDomain.Message);

        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Ssl.GetApproverEmailsAsync(Guid.Empty));

        // A certificate ID is enough: the API takes the domain and the package from the certificate.
        t.Handler.Respond(200, """{"mails":[]}""");
        await t.Client.SendAsync(new GetApproverEmailsRequest(CertificateId) { DomainName = "example.com" });

        Assert.Single(t.Handler.Requests);
    }

    public static TheoryData<string, Func<DomainNameApiClient, Task>> CertificateOperations() => new()
    {
        { "configure", c => c.Ssl.ConfigureAsync(new ConfigureCertificateRequest(Guid.Empty, "csr", Contact())) },
        { "check-status", c => c.Ssl.RefreshStatusAsync(Guid.Empty) },
        { "certificate", c => c.Ssl.GetAsync(Guid.Empty) },
        { "certificate-content", c => c.Ssl.GetContentAsync(Guid.Empty) },
        { "reissue", c => c.Ssl.ReissueAsync(new ReissueCertificateRequest(Guid.Empty)) },
        { "revoke", c => c.Ssl.RevokeAsync(Guid.Empty) },
        { "renew", c => c.Ssl.RenewAsync(Guid.Empty) },
        { "approvers", c => c.Ssl.GetApproverEmailsAsync(Guid.Empty) },
    };

    [Theory]
    [MemberData(nameof(CertificateOperations))]
    public async Task AnEmptyCertificateIdIsRefusedBeforeSending(string operation, Func<DomainNameApiClient, Task> call)
    {
        using var t = new TestClient();

        var e = await Assert.ThrowsAsync<ArgumentException>(() => call(t.Client));

        Assert.True(e.ParamName == "CertificateId", operation + " should name CertificateId, not " + e.ParamName);
        Assert.Contains("no longer addressed by domain name", e.Message);
        Assert.Contains("Ssl.ListAsync", e.Message);
        Assert.Empty(t.Handler.Requests);
    }

    [Fact]
    public async Task GenerateCsrNeedsTheFullSubject()
    {
        using var t = new TestClient();

        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Ssl.GenerateCsrAsync(new GenerateCsrRequest("example.com")));
    }
}
