using DomainNameApi.Dns;

namespace DomainNameApi.Tests.Features;

public class DnsClientTests
{
    [Fact]
    public async Task ListReadsRecordSets()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """
            [{"name":"www","ttl":3600,"type":"A","records":[{"content":"192.0.2.10","disabled":false},{"content":"192.0.2.11","disabled":true}]},
             {"name":"@","ttl":300,"type":"TXT","records":[{"content":"\"v=spf1 -all\""}]}]
            """);

        var sets = await t.Client.Dns.ListAsync("example.com");

        Assert.Equal("domains/zones", t.LastRequest.ApiPath);
        Assert.Equal("example.com", t.LastRequest.Query("domainName"));
        Assert.Equal(2, sets.Count);
        Assert.Equal(RecordType.A, sets[0].Type);
        Assert.Equal(new[] { "192.0.2.10" }, sets[0].Values);
        Assert.True(sets[0].Records[1].IsDisabled);
    }

    [Fact]
    public async Task AddWrapsTheRecordInZoneStruct()
    {
        using var t = new TestClient();
        t.Handler.Respond(200);

        await t.Client.Dns.AddAsync("example.com", "@", "txt", "v=spf1 include:_spf.example.net ~all", ttl: 600);

        Assert.Equal("POST", t.LastRequest.Method);
        Assert.Equal("example.com", t.LastRequest.Query("domainName"));
        var body = t.LastRequest.Json;
        Assert.Equal(new[] { "zoneStruct" }, body.Names());
        var zone = body.GetProperty("zoneStruct");
        Assert.Equal("@", zone.Str("name"));
        Assert.Equal("TXT", zone.Str("type"));
        Assert.Equal(600, zone.GetProperty("ttl").GetInt32());
        Assert.Equal("v=spf1 include:_spf.example.net ~all", zone.GetProperty("contents")[0].GetString());
    }

    [Fact]
    public async Task ReplaceTargetsTheRecordNameAndCanRename()
    {
        using var t = new TestClient();
        t.Handler.Respond(200).Respond(200);

        await t.Client.Dns.ReplaceAsync("example.com", "www", RecordType.A, ["192.0.2.20", "192.0.2.21"], ttl: 300);
        Assert.Equal("PUT", t.LastRequest.Method);
        Assert.Equal("www", t.LastRequest.Query("recordName"));
        Assert.Equal("www", t.LastRequest.Json.GetProperty("zoneStruct").Str("name"));
        Assert.Equal(2, t.LastRequest.Json.GetProperty("zoneStruct").GetProperty("contents").GetArrayLength());

        await t.Client.Dns.ReplaceAsync("example.com", "www", RecordType.A, ["192.0.2.20"], newName: "web");
        Assert.Equal("www", t.LastRequest.Query("recordName"));
        Assert.Equal("web", t.LastRequest.Json.GetProperty("zoneStruct").Str("name"));
    }

    [Fact]
    public async Task DeleteSendsTheValueInTheQuery()
    {
        using var t = new TestClient();
        t.Handler.Respond(200);

        await t.Client.Dns.DeleteAsync("example.com", "www", RecordType.A, "192.0.2.10");

        Assert.Equal("DELETE", t.LastRequest.Method);
        Assert.Equal("www", t.LastRequest.Query("Name"));
        Assert.Equal("192.0.2.10", t.LastRequest.Query("Record"));
        Assert.Equal("A", t.LastRequest.Query("RecordType"));
        Assert.Null(t.LastRequest.Body);
    }

    [Fact]
    public async Task ApplyHasNoBody()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"nameServersChanged":true,"previousNameServers":["ns1.other.net"],"currentNameServers":["ns1.platform.net"]}""");

        var result = await t.Client.Dns.ApplyAsync("example.com");

        Assert.Equal("domains/zones/apply", t.LastRequest.ApiPath);
        Assert.Null(t.LastRequest.Body);
        Assert.True(result.NameServersChanged);
        Assert.Equal("ns1.platform.net", Assert.Single(result.CurrentNameServers));
        Assert.Empty(result.SystemNameServers);
    }

    [Theory]
    [InlineData("@", "CNAME", "target.example.net", 3600)]
    [InlineData("", "CNAME", "target.example.net", 3600)]
    [InlineData("@", "CAA", "letsencrypt.org", 3600)]
    [InlineData("www", "A", "192.0.2.1", 0)]
    [InlineData("www", "A", "192.0.2.1", 86401)]
    [InlineData("www", "A", " ", 3600)]
    public async Task RejectsRecordsTheApiWouldRefuse(string name, string type, string value, int ttl)
    {
        using var t = new TestClient();

        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Dns.AddAsync("example.com", name, type, value, ttl));

        Assert.Empty(t.Handler.Requests);
    }

    [Fact]
    public async Task AcceptsAValidCaaRecord()
    {
        using var t = new TestClient();
        t.Handler.Respond(200);

        await t.Client.Dns.AddAsync("example.com", "@", RecordType.CAA, "0 issue \"letsencrypt.org\"");

        Assert.Single(t.Handler.Requests);
    }
}
