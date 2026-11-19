using DomainNameApi.Contacts;
using DomainNameApi.Dns;
using DomainNameApi.Dnssec;
using DomainNameApi.Domains;
using DomainNameApi.Ssl;

namespace DomainNameApi.Tests.Core;

public class SerializationTests
{
    [Theory]
    [InlineData("txt", "TXT")]
    [InlineData(" cname ", "CNAME")]
    [InlineData("dnskey", "DNSKey")]
    [InlineData("nsec3param", "NSEC3Param")]
    [InlineData("SPF", "SPF")]
    public void RecordTypesAreNormalised(string input, string expected)
    {
        RecordType type = input;

        Assert.Equal(expected, type.Value);
        Assert.Equal(expected, type.ToString());
    }

    [Fact]
    public void RecordTypesCompareIgnoringCase()
    {
        Assert.Equal(RecordType.TXT, new RecordType("txt"));
        Assert.True(RecordType.A == "a");
        Assert.True(RecordType.A != RecordType.AAAA);
        Assert.False(new RecordType("SPF").IsKnown);
        Assert.True(RecordType.CAA.IsKnown);
    }

    [Fact]
    public void TextEnumsRoundTripAsJsonStrings()
    {
        var json = JsonSerializer.Serialize(new { type = RecordType.MX, role = ContactType.Admin, method = ValidationMethod.Http });
        Assert.Equal("""{"type":"MX","role":"Admin","method":"Http"}""", json);

        var set = JsonSerializer.Deserialize<DnsRecordSet>("""{"name":"@","type":"txt","ttl":300,"records":[]}""", new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        Assert.Equal(RecordType.TXT, set.Type);

        var empty = JsonSerializer.Deserialize<DnsRecordSet>("""{"type":null}""", new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        Assert.Equal("", empty.Type.Value);
    }

    [Fact]
    public void ContactTypesKnowAllFourRoles()
    {
        Assert.Equal(new[] { "Registrant", "Admin", "Tech", "Billing" }, ContactType.All.Select(r => r.Value));
        Assert.Equal(ContactType.Tech, (ContactType)"TECH");
        Assert.False(((ContactType)"Owner").IsKnown);
    }

    [Theory]
    [InlineData("192.0.2.10", "v4")]
    [InlineData(" 2001:db8::10 ", "v6")]
    public void DetectsIpVersions(string address, string version)
    {
        var parsed = HostIpAddress.Parse(address);

        Assert.Equal(address.Trim(), parsed.IpAddress);
        Assert.Equal(version, parsed.IpVersion.Value);
    }

    [Fact]
    public void RejectsInvalidIpAddresses() => Assert.Throws<ArgumentException>(() => HostIpAddress.Parse("ns1.example.com"));

    [Theory]
    [InlineData("12345 13 2 49FD46E6C4B45C55D4AC69CBD3CD34AC1AFE51DE")]
    [InlineData("example.com. 3600 IN DS 12345 13 2 49FD46E6C4B45C55 D4AC69CBD3CD34AC1AFE51DE")]
    public void ParsesDsRecords(string text)
    {
        var ds = DsRecordData.Parse(text);

        Assert.Equal(12345, ds.KeyTag);
        Assert.Equal(DnssecAlgorithm.EcdsaP256Sha256, ds.Algorithm);
        Assert.Equal(DsDigestType.Sha256, ds.DigestType);
        Assert.Equal("49FD46E6C4B45C55D4AC69CBD3CD34AC1AFE51DE", ds.Digest);
    }

    [Fact]
    public void RejectsInvalidDsRecords() => Assert.Throws<FormatException>(() => DsRecordData.Parse("12345 x 2 ABCD"));

    [Fact]
    public void QueryParametersSkipNullsAndRepeatLists()
    {
        var query = new QueryParameters()
            .Add("Name", (string?)null)
            .Add("Empty", "")
            .Add("Text", "a b&c")
            .Add("Count", 5)
            .Add("Missing", (int?)null)
            .Add("Flag", true)
            .Add("Amount", 12.5m)
            .Add("Status", (DomainStatus?)DomainStatus.PendingDelete)
            .AddEach("Types", new[] { Accounts.TransactionType.Register, Accounts.TransactionType.Renew })
            .AddCommaSeparated("Ids", new[] { DomainStatus.Active, DomainStatus.Deleted })
            .Add("When", new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero));

        Assert.Equal(
            "Text=a%20b%26c&Count=5&Flag=true&Amount=12.5&Status=7&Types=1&Types=2&Ids=0%2C8&When=2026-01-02T03%3A04%3A05.0000000%2B00%3A00",
            query.ToString());
    }

    [Fact]
    public void DocumentUploadsAreBase64()
    {
        var upload = DocumentUpload.FromBytes(DocumentType.IdentityCard, "id.pdf", [1, 2, 3]);

        Assert.Equal("AQID", upload.FileContent);
        Assert.Equal("id.pdf", upload.FileName);
    }

    [Fact]
    public void FullChainJoinsCertificateAndIntermediates()
    {
        var content = new CertificateContent
        {
            Certificate = "-----CERT-----\n",
            IntermediateCertificate1 = "-----CA1-----",
            IntermediateCertificate2 = "",
            RootCertificate = "-----ROOT-----",
        };

        Assert.Equal("-----CERT-----\n-----CA1-----\n", content.FullChain);
    }

    [Fact]
    public void ContactValidationNamesEveryMissingField()
    {
        var contact = new ContactInfo { FirstName = "Ada", Email = "ada@example.com" };

        var e = Assert.Throws<ArgumentException>(() => contact.EnsureComplete());

        Assert.Contains("LastName", e.Message);
        Assert.Contains("PhoneCountryCode", e.Message);
        Assert.DoesNotContain("FirstName", e.Message);
        Assert.DoesNotContain("CompanyName", e.Message);
    }
}
