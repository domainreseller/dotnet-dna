using DomainNameApi.Domains;

namespace DomainNameApi.Tests.Core;

public class RetryTests
{
    private const string Account = """{"resellerName":"Acme"}""";
    private const string Renewed = """{"success":true,"status":"Active"}""";

    [Fact]
    public async Task RateLimitedRequestsAreRetriedEvenWhenBillable()
    {
        using var t = new TestClient();
        t.Handler.Respond(429, "", ("Retry-After", "1")).Respond(200, Renewed);

        var result = await t.Client.Domains.RenewAsync("example.com");

        Assert.True(result.Success);
        Assert.Equal(2, t.Handler.Requests.Count);
        Assert.Equal(TimeSpan.FromMilliseconds(1050), Assert.Single(t.Delays));
    }

    [Fact]
    public async Task GivesUpAfterMaxRetries()
    {
        using var t = new TestClient(o => o.MaxRetries = 2);
        t.Handler.Respond(429, "", ("Retry-After", "2")).Respond(429, "", ("Retry-After", "2")).Respond(429, "", ("Retry-After", "2"));

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Equal(ApiErrorKind.RateLimited, e.Kind);
        Assert.Equal("RATE_LIMITED", e.Code);
        Assert.Equal(TimeSpan.FromSeconds(2), e.RetryAfter);
        Assert.Equal(3, t.Handler.Requests.Count);
    }

    [Fact]
    public async Task DoesNotWaitLongerThanMaxRetryDelay()
    {
        using var t = new TestClient(o => o.MaxRetryDelay = TimeSpan.FromSeconds(5));
        t.Handler.Respond(429, "", ("Retry-After", "60"));

        await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Single(t.Handler.Requests);
        Assert.Empty(t.Delays);
    }

    [Theory]
    [InlineData(502)]
    [InlineData(503)]
    [InlineData(504)]
    public async Task ReadsAreRetriedOnGatewayErrors(int status)
    {
        using var t = new TestClient();
        t.Handler.Respond(status, "").Respond(200, Account);

        await t.Client.Account.GetAsync();

        Assert.Equal(2, t.Handler.Requests.Count);
        Assert.Single(t.Delays);
    }

    [Fact]
    public async Task BillableRequestsAreNotRetriedOnGatewayErrors()
    {
        using var t = new TestClient();
        t.Handler.Respond(503, "");

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Domains.RenewAsync("example.com"));

        Assert.Equal(ApiErrorKind.Server, e.Kind);
        Assert.Single(t.Handler.Requests);
    }

    [Fact]
    public async Task IdempotentWritesAreRetried()
    {
        using var t = new TestClient();
        t.Handler.Respond(503, "").Respond(200);

        await t.Client.Domains.SetNameServersAsync("example.com", ["ns1.example.net", "ns2.example.net"]);

        Assert.Equal(2, t.Handler.Requests.Count);
    }

    [Fact]
    public async Task ReadsAreRetriedAfterNetworkErrors()
    {
        using var t = new TestClient();
        t.Handler.Fail(new HttpRequestException("reset")).Respond(200, Account);

        var account = await t.Client.Account.GetAsync();

        Assert.Equal("Acme", account.ResellerName);
        Assert.Equal(2, t.Handler.Requests.Count);
        Assert.InRange(Assert.Single(t.Delays), TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(750));
    }

    [Fact]
    public async Task WritesAreNotRetriedAfterNetworkErrors()
    {
        using var t = new TestClient();
        t.Handler.Fail(new HttpRequestException("reset"));

        await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Domains.LockAsync("example.com"));

        Assert.Single(t.Handler.Requests);
    }

    [Fact]
    public async Task MaxRetriesZeroTurnsRetriesOff()
    {
        using var t = new TestClient(o => o.MaxRetries = 0);
        t.Handler.Respond(429, "", ("Retry-After", "1"));

        await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Single(t.Handler.Requests);
    }

    [Fact]
    public async Task ResponseReportsTheAttempts()
    {
        using var t = new TestClient();
        t.Handler.Respond(429, "").Respond(200, Account);

        var response = await t.Client.SendWithResponseAsync(new Accounts.GetAccountRequest());

        Assert.Equal(2, response.Attempts);
    }
}
