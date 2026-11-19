using DomainNameApi.Contacts;

namespace DomainNameApi.Tests.Features;

public class ContactsClientTests
{
    internal static ContactInfo Ada() => new()
    {
        FirstName = "Ada",
        LastName = "Lovelace",
        Email = "ada@example.com",
        Address = "1 Example Street",
        City = "Istanbul",
        State = "Istanbul",
        PostalCode = "34000",
        Country = "TR",
        PhoneCountryCode = "90",
        Phone = "5551234567",
    };

    [Fact]
    public async Task CreateSendsTheContactUnderContact()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"handle":"DNA-42","isSuccess":true,"operationMessage":"ok"}""");

        var result = await t.Client.Contacts.CreateAsync(Ada());

        Assert.Equal("POST", t.LastRequest.Method);
        Assert.Equal("contacts", t.LastRequest.ApiPath);
        var contact = t.LastRequest.Json.GetProperty("contact");
        Assert.Equal("ada@example.com", contact.Str("eMail"));
        Assert.Equal("90", contact.Str("phoneCountryCode"));
        Assert.False(contact.Has("discloseFlag")); // null: the registry default applies
        Assert.Equal("DNA-42", result.Handle);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateRefusesAnIncompleteContact()
    {
        using var t = new TestClient();

        var e = await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Contacts.CreateAsync(new ContactInfo { FirstName = "Ada" }));

        Assert.Contains("Email", e.Message);
        Assert.Empty(t.Handler.Requests);
    }

    [Fact]
    public async Task GetEscapesTheHandleAndReadsVerifications()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """
            {"contact":{"firstName":"Ada","eMail":"ada@example.com","contactType":"registrant","handle":"DNA/1"},
             "verificationObjects":[{"fieldType":"Email","fieldValue":"ada@example.com","isUserVerified":true,"userVerificationDate":"2026-01-02T03:04:05"}]}
            """);

        var details = await t.Client.Contacts.GetAsync("DNA/1");

        Assert.Equal("/api/v1/contacts/DNA%2F1", t.LastRequest.Uri.AbsolutePath);
        Assert.Equal("ada@example.com", details.Contact!.Email);
        Assert.Equal(ContactType.Registrant, details.Contact.ContactType);
        var verification = Assert.Single(details.Verifications);
        Assert.True(verification.IsUserVerified);
        Assert.Equal(new DateTime(2026, 1, 2, 3, 4, 5), verification.UserVerificationDate);
    }

    [Fact]
    public async Task UpdatePutsTheContactAtTheHandlePath()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"handle":"DNA-1","contactUpdateSuccess":true,"stats":{"totalDomainCount":3,"successCount":2,"failedCount":1},"affectedDomains":[{"domainName":"example.com","status":"Updated"}]}""");

        var result = await t.Client.Contacts.UpdateAsync("DNA-1", Ada());

        Assert.Equal("PUT", t.LastRequest.Method);
        Assert.Equal("contacts/DNA-1", t.LastRequest.ApiPath);
        Assert.Equal(new[] { "contact" }, t.LastRequest.Json.Names());
        Assert.Equal(3, result.Stats!.TotalDomainCount);
        Assert.Equal("example.com", Assert.Single(result.AffectedDomains).DomainName);
    }

    [Fact]
    public async Task VerificationUsesTheHandleCodeQuery()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"results":[{"fieldType":"Phone","isUserVerified":false}]}""");

        var results = await t.Client.Contacts.GetVerificationAsync("DNA-1");

        Assert.Equal("contacts/verification/check", t.LastRequest.ApiPath);
        Assert.Equal("DNA-1", t.LastRequest.Query("HandleCode"));
        Assert.False(Assert.Single(results).IsUserVerified);
    }

    [Fact]
    public async Task ResendSendsTheHandleCode()
    {
        using var t = new TestClient();
        t.Handler.Respond(200);

        await t.Client.Contacts.ResendVerificationAsync("DNA-1");

        Assert.Equal("contacts/verification/request", t.LastRequest.ApiPath);
        Assert.Equal("DNA-1", t.LastRequest.Json.Str("handleCode"));
    }

    [Fact]
    public void ForAllRolesCopiesTheContactFourTimes()
    {
        var contacts = DomainContact.ForAllRoles(Ada());

        Assert.Equal(new[] { "Registrant", "Admin", "Tech", "Billing" }, contacts.Select(c => c.ContactType.Value));
        Assert.All(contacts, c => Assert.Equal("Ada", c.FirstName));
        Assert.Equal(new[] { "DNA-1", "DNA-1", "DNA-1", "DNA-1" }, ContactHandle.ForAllRoles("DNA-1").Select(h => h.Handle));
    }
}
