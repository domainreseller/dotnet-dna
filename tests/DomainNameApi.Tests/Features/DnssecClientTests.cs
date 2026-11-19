using DomainNameApi.Dnssec;

namespace DomainNameApi.Tests.Features;

public class DnssecClientTests
{
    private static readonly DsRecordData Record = DsRecordData.Parse("12345 13 2 49FD46E6C4B45C55D4AC69CBD3CD34AC1AFE51DE");

    [Fact]
    public async Task GetReadsTheState()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """
            {"domainName":"example.com","isDnssecAvailable":true,"status":2,"delegationSigned":true,
             "dsRecords":[{"keyTag":12345,"algorithm":13,"digestType":2,"digest":"49FD","status":1}],
             "allowedAlgorithms":[8,13],"allowedDigestTypes":[2],"maxDsRecordCount":4}
            """);

        var info = await t.Client.Dnssec.GetAsync("example.com");

        Assert.Equal("domains/dns/dnssec", t.LastRequest.ApiPath);
        Assert.Equal(DnssecStatus.Signed, info.Status);
        Assert.True(info.IsDelegationSigned);
        Assert.Equal(DsRecordStatus.Active, Assert.Single(info.DsRecords).Status);
        Assert.Equal(new[] { DnssecAlgorithm.RsaSha256, DnssecAlgorithm.EcdsaP256Sha256 }, info.AllowedAlgorithms);
    }

    [Fact]
    public async Task AddSendsNumbers()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"items":[{"keyTag":12345,"operationType":0,"isSuccess":true,"isPending":true}]}""");

        var results = await t.Client.Dnssec.AddAsync("example.com", Record);

        var record = Assert.Single(t.LastRequest.Json.GetProperty("records").EnumerateArray());
        Assert.Equal(12345, record.GetProperty("keyTag").GetInt32());
        Assert.Equal(13, record.GetProperty("algorithm").GetInt32());
        Assert.Equal(2, record.GetProperty("digestType").GetInt32());
        Assert.Equal("example.com", t.LastRequest.Json.Str("domainName"));
        Assert.True(Assert.Single(results).IsPending);
    }

    [Fact]
    public async Task RemoveAndDisable()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"keyTag":12345,"operationType":1,"isSuccess":true}""").Respond(200, """{"operationType":2,"isSuccess":true}""");

        var removed = await t.Client.Dnssec.RemoveAsync("example.com", Record);
        Assert.Equal("DELETE", t.LastRequest.Method);
        Assert.Equal("12345", t.LastRequest.Query("KeyTag"));
        Assert.Equal("13", t.LastRequest.Query("Algorithm"));
        Assert.Equal("2", t.LastRequest.Query("DigestType"));
        Assert.Equal(Record.Digest, t.LastRequest.Query("Digest"));
        Assert.Equal(DnssecOperationType.Remove, removed.OperationType);

        var disabled = await t.Client.Dnssec.DisableAsync("example.com");
        Assert.Equal("domains/dns/dnssec/disable", t.LastRequest.ApiPath);
        Assert.Equal(DnssecOperationType.RemoveAll, disabled.OperationType);
    }
}
