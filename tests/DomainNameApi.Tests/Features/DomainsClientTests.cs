using DomainNameApi.Contacts;
using DomainNameApi.Domains;

namespace DomainNameApi.Tests.Features;

public class DomainsClientTests
{
    private const string DomainInfo = """
        {"domainName":"example.com","isIdnName":false,"startDate":"2024-03-15T10:20:30","expirationDate":"2027-03-15T10:20:30",
         "transferDate":null,"updatedDate":"2026-10-01T08:00:00","resellerId":"00000000-0000-4000-8000-000000000001",
         "authCode":"Ab1!xyZ9","lockStatus":true,"privacyProtectionStatus":false,"remainingDay":126,"status":"Active",
         "nameservers":["ns1.example.net","ns2.example.net"],
         "hosts":[{"name":"ns1.example.com","ipAddresses":[{"ipAddress":"192.0.2.10","ipVersion":"v4"}]},{"name":"ns2.example.com","ipAddresses":null}],
         "contacts":[{"firstName":"Ada","eMail":"ada@example.com","contactType":"Registrant","handle":"DNA-R-1"},
                     {"firstName":"Grace","eMail":"grace@example.com","contactType":"Admin","handle":"DNA-A-2"}],
         "additionalAttributes":{"trabisType":"individual"}}
        """;

    private const string Operation = """{"success":true,"message":"done","domainId":"7d9f1a52-3c1e-4c9e-9a51-6b8c2f3e4d10","status":"Active","expirationDate":"2028-03-15T10:20:30"}""";

    [Fact]
    public async Task SearchPostsTheDomainAndCurrency()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"success":true,"code":"1000","info":{"domainName":"example.com","tld":"COM","status":"AVAILABLE","price":11.99,"currency":"USD","period":1,"isPremium":false}}""");

        var result = await t.Client.Domains.SearchAsync("example.com", "USD");

        Assert.Equal("POST", t.LastRequest.Method);
        Assert.Equal("domains/search", t.LastRequest.ApiPath);
        Assert.Equal("example.com", t.LastRequest.Json.Str("domainName"));
        Assert.Equal("USD", t.LastRequest.Json.Str("currency"));
        Assert.True(result.Info!.IsAvailable);
        Assert.Equal(11.99m, result.Info.Price);
    }

    [Fact]
    public async Task BulkSearchSendsAnArray()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """
            {"success":true,"infos":[{"domainName":"a.com","status":"AVAILABLE"},{"domainName":"b.com","status":"NOTAVAILABLE","reason":"Domain exists"}]}
            """);

        var result = await t.Client.Domains.BulkSearchAsync([" a.com", "b.com"], "TRY");

        var body = t.LastRequest.Json;
        Assert.Equal(JsonValueKind.Array, body.ValueKind);
        Assert.Equal(new[] { "a.com", "b.com" }, body.EnumerateArray().Select(e => e.Str("domainName")));
        Assert.All(body.EnumerateArray(), e => Assert.Equal("TRY", e.Str("currency")));
        Assert.Equal("a.com", Assert.Single(result.Available).DomainName);
        Assert.Equal("Domain exists", result.Results[1].Reason);
    }

    [Fact]
    public async Task BulkSearchNeedsAtLeastOneDomain()
    {
        using var t = new TestClient();

        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Domains.BulkSearchAsync([]));
    }

    [Fact]
    public async Task ListSendsStatusIdsAsACommaSeparatedList()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"totalCount":1,"items":[{"domainName":"example.com","status":7,"lockStatus":true,"remainingDay":3,"expirationDate":"2026-11-22T00:00:00","nameservers":[{"name":"ns1.example.net","isChildnameServer":false}]}]}""");

        var page = await t.Client.Domains.ListAsync(new ListDomainsRequest
        {
            DomainName = "example",
            Statuses = { DomainStatus.Active, DomainStatus.PendingDelete },
        });

        Assert.Equal("domains", t.LastRequest.ApiPath);
        Assert.Equal("example", t.LastRequest.Query("DomainName"));
        Assert.Equal("0,7", t.LastRequest.Query("StatusIds"));
        var domain = Assert.Single(page.Items);
        Assert.Equal(DomainStatus.PendingDelete, domain.Status);
        Assert.True(domain.IsLocked);
        Assert.Equal(3, domain.RemainingDays);
        Assert.Equal("ns1.example.net", Assert.Single(domain.NameServers).Name);
    }

    [Fact]
    public async Task GetReadsTheFullDetails()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, DomainInfo);

        var details = await t.Client.Domains.GetAsync("example.com");

        Assert.Equal("domains/info", t.LastRequest.ApiPath);
        Assert.Equal("example.com", t.LastRequest.Query("DomainName"));
        Assert.Equal(new DateTime(2027, 3, 15, 10, 20, 30), details.ExpirationDate);
        Assert.Null(details.TransferDate);
        Assert.True(details.IsLocked);
        Assert.False(details.IsPrivacyProtected);
        Assert.Equal(126, details.RemainingDays);
        Assert.Equal(new[] { "ns1.example.net", "ns2.example.net" }, details.NameServers);
        Assert.Equal(IpVersion.V4, details.Hosts[0].IpAddresses[0].IpVersion);
        Assert.Empty(details.Hosts[1].IpAddresses); // null in the answer
        Assert.Equal("grace@example.com", details.GetContact(ContactType.Admin)!.Email);
        Assert.Null(details.GetContact(ContactType.Billing));
        Assert.Equal("individual", details.AdditionalAttributes!["trabisType"].GetString());
    }

    [Fact]
    public async Task RegisterSendsAllFourContactRoles()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Operation);

        var result = await t.Client.Domains.RegisterAsync("example.com", ContactsClientTests.Ada(), ["ns1.example.net", "ns2.example.net"], years: 2);

        Assert.Equal("domains/register-with-contacts", t.LastRequest.ApiPath);
        var body = t.LastRequest.Json;
        Assert.Equal("example.com", body.Str("domainName"));
        Assert.Equal(2, body.GetProperty("period").GetInt32());
        Assert.Equal(2, body.GetProperty("nameServers").GetArrayLength());
        Assert.Equal(new[] { "Registrant", "Admin", "Tech", "Billing" }, body.GetProperty("contacts").EnumerateArray().Select(c => c.Str("contactType")));
        Assert.False(body.GetProperty("useTrusteeContact").GetBoolean());
        Assert.False(body.Has("tldAttributes"));
        Assert.True(result.Success);
        Assert.Equal(Guid.Parse("7d9f1a52-3c1e-4c9e-9a51-6b8c2f3e4d10"), result.DomainId);
    }

    [Fact]
    public async Task RegisterRefusesMissingRoles()
    {
        using var t = new TestClient();
        var request = new RegisterDomainRequest("example.com")
        {
            NameServers = { "ns1.example.net" },
            Contacts = { new DomainContact(ContactsClientTests.Ada(), ContactType.Registrant) },
        };

        var e = await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Domains.RegisterAsync(request));

        Assert.Contains("Admin, Tech, Billing", e.Message);
        Assert.Empty(t.Handler.Requests);
    }

    [Fact]
    public async Task RegisterWithATrusteeNeedsNoOtherRoles()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Operation);
        var request = new RegisterDomainRequest("example.de")
        {
            NameServers = { "ns1.example.net", "ns2.example.net" },
            Contacts = { new DomainContact(ContactsClientTests.Ada(), ContactType.Registrant) },
            UseTrusteeContact = true,
            TldAttributes = new() { ["trustee"] = "yes" },
        };

        await t.Client.Domains.RegisterAsync(request);

        Assert.True(t.LastRequest.Json.GetProperty("useTrusteeContact").GetBoolean());
        Assert.Equal("yes", t.LastRequest.Json.GetProperty("tldAttributes").Str("trustee"));
    }

    [Fact]
    public async Task RegisterWithHandleUsesItForEveryRole()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Operation);

        await t.Client.Domains.RegisterWithHandleAsync("example.com", "DNA-7", ["ns1.example.net", "ns2.example.net"]);

        Assert.Equal("domains/register", t.LastRequest.ApiPath);
        var handles = t.LastRequest.Json.GetProperty("handles").EnumerateArray().ToList();
        Assert.Equal(4, handles.Count);
        Assert.All(handles, h => Assert.Equal("DNA-7", h.Str("handle")));
        Assert.Equal(1, t.LastRequest.Json.GetProperty("period").GetInt32());
    }

    [Fact]
    public async Task RenewTransferAndChecks()
    {
        using var t = new TestClient();
        t.Handler
            .Respond(200, Operation)
            .Respond(200, """{"renewabilityStatus":true,"renewalLock":false,"isPremium":true,"premiumPrice":120.5,"premiumCurrency":"USD"}""")
            .Respond(200, Operation)
            .Respond(200, """{"transferAvailabilityStatus":false,"authCodeIsValid":false,"transferLock":true,"domainStatuses":["clientTransferProhibited"]}""")
            .Respond(200, """{"domainName":"example.com","status":"pending","requestedDate":"2026-11-19"}""");

        await t.Client.Domains.RenewAsync("example.com", years: 3);
        Assert.Equal("domains/renew", t.LastRequest.ApiPath);
        Assert.Equal(3, t.LastRequest.Json.GetProperty("period").GetInt32());

        var renewal = await t.Client.Domains.CheckRenewalAsync("example.com");
        Assert.Equal("domains/renew/check", t.LastRequest.ApiPath);
        Assert.True(renewal.IsRenewable);
        Assert.Equal(120.5m, renewal.PremiumPrice);

        await t.Client.Domains.TransferAsync("example.com", "EPP-CODE");
        Assert.Equal("domains/transfer", t.LastRequest.ApiPath);
        Assert.Equal("EPP-CODE", t.LastRequest.Json.Str("authCode"));
        Assert.False(t.LastRequest.Json.Has("contacts"));

        var check = await t.Client.Domains.CheckTransferAsync("example.com", "EPP-CODE");
        Assert.Equal("domains/transfers/check", t.LastRequest.ApiPath);
        Assert.False(check.IsTransferable);
        Assert.True(check.HasTransferLock);
        Assert.Equal("clientTransferProhibited", Assert.Single(check.DomainStatuses));

        var status = await t.Client.Domains.GetTransferStatusAsync("example.com");
        Assert.Equal("domains/transfers/query", t.LastRequest.ApiPath);
        Assert.Equal("pending", status.Status);
    }

    [Fact]
    public async Task LockPrivacyRestoreAndAuthCode()
    {
        using var t = new TestClient();
        t.Handler.Respond(200).Respond(200).Respond(200).Respond(200).Respond(200, """{"domainName":"example.com","authCode":"New!Code1"}""");

        await t.Client.Domains.LockAsync("example.com");
        Assert.Equal("domains/lock", t.LastRequest.ApiPath);

        await t.Client.Domains.UnlockAsync("example.com");
        Assert.Equal("domains/unlock", t.LastRequest.ApiPath);

        await t.Client.Domains.SetPrivacyProtectionAsync("example.com", enabled: true);
        Assert.Equal("domains/privacy", t.LastRequest.ApiPath);
        Assert.True(t.LastRequest.Json.GetProperty("privacyStatus").GetBoolean());

        await t.Client.Domains.RestoreAsync("example.com");
        Assert.Equal("domains/restore", t.LastRequest.ApiPath);

        var code = await t.Client.Domains.UpdateAuthCodeAsync("example.com");
        Assert.Equal("PUT", t.LastRequest.Method);
        Assert.Equal("domains/auth", t.LastRequest.ApiPath);
        Assert.Equal("New!Code1", code.AuthCode);
    }

    [Fact]
    public async Task SetNameServersSendsTheDnssecModeAsANumber()
    {
        using var t = new TestClient();
        t.Handler.Respond(200).Respond(200);

        await t.Client.Domains.SetNameServersAsync("example.com", ["ns1.example.net", "ns2.example.net"], DnssecMode.DisableDnssec);
        Assert.Equal("PUT", t.LastRequest.Method);
        Assert.Equal("domains/dns/name-server", t.LastRequest.ApiPath);
        Assert.Equal(2, t.LastRequest.Json.GetProperty("dnssecMode").GetInt32());

        await t.Client.Domains.SetNameServersAsync("example.com", ["ns1.example.net"]);
        Assert.False(t.LastRequest.Json.Has("dnssecMode"));
    }

    [Fact]
    public async Task ContactChanges()
    {
        using var t = new TestClient();
        t.Handler.Respond(200).Respond(200);

        await t.Client.Domains.UpdateContactsAsync("example.com", DomainContact.ForAllRoles(ContactsClientTests.Ada()));
        Assert.Equal("domains/contacts/update", t.LastRequest.ApiPath);
        Assert.Equal(4, t.LastRequest.Json.GetProperty("contacts").GetArrayLength());

        await t.Client.Domains.SetContactHandlesAsync("example.com", [new ContactHandle(ContactType.Tech, "DNA-9")]);
        Assert.Equal("domains/contacts/update-with-handle", t.LastRequest.ApiPath);
        var item = Assert.Single(t.LastRequest.Json.GetProperty("handleItems").EnumerateArray());
        Assert.Equal("Tech", item.Str("contactType"));
        Assert.Equal("DNA-9", item.Str("handle"));
    }

    [Fact]
    public async Task Forwarding()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"redirectAlias":"https://example.net","redirectType":"frame"}""").Respond(200).Respond(200);

        var forwarding = await t.Client.Domains.Forwarding.GetAsync("example.com");
        Assert.Equal("domains/forwards", t.LastRequest.ApiPath);
        Assert.Equal("example.com", t.LastRequest.Query("domainName"));
        Assert.Equal("https://example.net", forwarding.RedirectAddress);
        Assert.Equal(ForwardType.Frame, forwarding.ForwardType);

        await t.Client.Domains.Forwarding.SetAsync("example.com", "https://example.org");
        Assert.Equal("Standard", t.LastRequest.Json.Str("forwardType"));
        Assert.Equal("https://example.org", t.LastRequest.Json.Str("redirectAddress"));

        await t.Client.Domains.Forwarding.DeleteAsync("example.com");
        Assert.Equal("DELETE", t.LastRequest.Method);
        Assert.Equal("example.com", t.LastRequest.Query("DomainName"));
    }

    [Fact]
    public async Task ChildNameServersDetectIpVersions()
    {
        using var t = new TestClient();
        t.Handler.Respond(200).Respond(200).Respond(200);

        await t.Client.Domains.ChildNameServers.AddAsync("example.com", "ns1.example.com", "192.0.2.10", "2001:db8::10");
        Assert.Equal("POST", t.LastRequest.Method);
        Assert.Equal("domains/dns/host", t.LastRequest.ApiPath);
        var addresses = t.LastRequest.Json.GetProperty("ipAddresses").EnumerateArray().ToList();
        Assert.Equal(new[] { "v4", "v6" }, addresses.Select(a => a.Str("ipVersion")));
        Assert.Equal("ns1.example.com", t.LastRequest.Json.Str("hostName"));

        await t.Client.Domains.ChildNameServers.UpdateAsync("example.com", "ns1.example.com", ["192.0.2.20"], newHostName: "ns3.example.com");
        Assert.Equal("PUT", t.LastRequest.Method);
        Assert.Equal("ns3.example.com", t.LastRequest.Json.Str("newHostName"));

        await t.Client.Domains.ChildNameServers.DeleteAsync("example.com", "ns1.example.com");
        Assert.Equal("DELETE", t.LastRequest.Method);
        Assert.Equal("ns1.example.com", t.LastRequest.Query("HostName"));
    }

    [Fact]
    public async Task ChildNameServersRejectBadAddressesBeforeSending()
    {
        using var t = new TestClient();

        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Domains.ChildNameServers.AddAsync("example.com", "ns1.example.com", "not-an-ip"));
        Assert.Empty(t.Handler.Requests);
    }

    [Fact]
    public async Task Documents()
    {
        using var t = new TestClient();
        t.Handler
            .Respond(200, """{"totalCount":1,"items":[{"name":"id.pdf","type":1,"status":3,"operationType":1,"uploadedDate":"2026-11-01T00:00:00"}]}""")
            .Respond(200, """{"uploaded":[{"name":"tax.pdf","size":3,"type":3}],"duplicates":[],"failed":[],"totalDocumentCount":2}""");

        var page = await t.Client.Domains.Documents.ListAsync(new ListDocumentsRequest("example.com.tr") { DocumentTypes = { DocumentType.IdentityCard, DocumentType.TaxCertificate } });
        Assert.Equal("domains/documents", t.LastRequest.ApiPath);
        Assert.Equal("1,3", t.LastRequest.Query("DocumentTypes"));
        Assert.Equal(DocumentStatus.Approved, Assert.Single(page.Items).Status);

        var result = await t.Client.Domains.Documents.UploadAsync("example.com.tr", [DocumentUpload.FromBytes(DocumentType.TaxCertificate, "tax.pdf", [7, 8, 9])]);
        var document = Assert.Single(t.LastRequest.Json.GetProperty("documents").EnumerateArray());
        Assert.Equal(3, document.GetProperty("documentType").GetInt32());
        Assert.Equal("BwgJ", document.Str("fileContent"));
        Assert.Equal(2, result.TotalDocumentCount);
    }
}
