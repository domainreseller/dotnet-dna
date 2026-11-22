using DomainNameApi.Accounts;

namespace DomainNameApi.Tests.Features;

public class AccountClientTests
{
    private static string Page(long total, params int[] periods) =>
        JsonSerializer.Serialize(new
        {
            totalCount = total,
            items = periods.Select(p => new { traceId = "t" + p, type = 2, status = 1, chargeType = 1, amount = 9.99m, amountCurrency = "USD", period = p, creationTime = "2026-11-01T10:00:00Z" }),
        });

    [Fact]
    public async Task GetProfileReadsTheResellerProfile()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """
            {"id":"00000000-0000-4000-8000-000000000001","resellerAutoId":4821,"name":"example","companyName":"Example Ltd",
             "resellerGroupName":"Gold","accountCurrency":2,"status":0,"isSubReseller":false,"isBalanceSuspended":true,"isMcpEnabled":true}
            """);

        var profile = await t.Client.Account.GetProfileAsync();

        Assert.Equal("GET", t.LastRequest.Method);
        Assert.Equal("resellers/me", t.LastRequest.ApiPath);
        Assert.Empty(t.LastRequest.QueryParameters);
        Assert.Null(t.LastRequest.Body);

        Assert.Equal(Guid.Parse(TestClient.ResellerId), profile.Id);
        Assert.Equal(4821, profile.ResellerAutoId);
        Assert.Equal("example", profile.Name);
        Assert.Equal("Example Ltd", profile.CompanyName);
        Assert.Equal("Gold", profile.ResellerGroupName);
        Assert.Equal(AccountCurrency.Usd, profile.AccountCurrency);
        Assert.Equal(ResellerStatus.Active, profile.Status);
        Assert.False(profile.IsSubReseller);
        Assert.True(profile.IsBalanceSuspended);
        Assert.True(profile.IsMcpEnabled);
    }

    [Theory]
    [InlineData(1, AccountCurrency.Try)]
    [InlineData(2, AccountCurrency.Usd)]
    [InlineData(3, AccountCurrency.Euro)]
    public async Task ProfileCurrenciesUseTheApiNumbers(int number, AccountCurrency expected)
    {
        using var t = new TestClient();
        t.Handler.Respond(200, "{\"accountCurrency\":" + number + "}");

        Assert.Equal(expected, (await t.Client.Account.GetProfileAsync()).AccountCurrency);
    }

    [Theory]
    [InlineData(0, ResellerStatus.Active)]
    [InlineData(1, ResellerStatus.WaitingForApproval)]
    [InlineData(2, ResellerStatus.Suspend)]
    [InlineData(3, ResellerStatus.WaitingForCompletionWizard)]
    public async Task ProfileStatusesUseTheApiNumbers(int number, ResellerStatus expected)
    {
        using var t = new TestClient();
        t.Handler.Respond(200, "{\"status\":" + number + "}");

        Assert.Equal(expected, (await t.Client.Account.GetProfileAsync()).Status);
    }

    [Fact]
    public async Task GetProfileIsAReadAndWorksInReadOnlyMode()
    {
        using var t = new TestClient(o => o.ReadOnly = true);
        t.Handler.Respond(200, "{}");

        var profile = await t.Client.Account.GetProfileAsync();

        Assert.Equal(Guid.Empty, profile.Id);
        Assert.Null(profile.Name);
    }

    [Fact]
    public async Task ListTransactionsSendsTheFilters()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Page(1, 1));

        var page = await t.Client.Account.ListTransactionsAsync(new ListTransactionsRequest
        {
            TransactionTypes = { TransactionType.Register, TransactionType.Renew },
            ChargeType = ChargeType.Subtract,
            StartDate = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero),
            MaxResultCount = 50,
            Sorting = "creationTime desc",
        });

        var request = t.LastRequest;
        Assert.Equal("deposit/transactions", request.ApiPath);
        Assert.Equal(new[] { "1", "2" }, request.QueryAll("TransactionTypes"));
        Assert.Equal("1", request.Query("ChargeType"));
        Assert.Equal("2026-11-01T00:00:00.0000000+00:00", request.Query("StartDate"));
        Assert.Equal("50", request.Query("MaxResultCount"));
        Assert.Equal("creationTime desc", request.Query("Sorting"));
        Assert.Null(request.Query("SkipCount"));

        var item = Assert.Single(page.Items);
        Assert.Equal(TransactionType.Renew, item.Type);
        Assert.Equal(TransactionStatus.Success, item.Status);
        Assert.Equal(ChargeType.Subtract, item.ChargeType);
        Assert.Equal(9.99m, item.Amount);
        Assert.Equal(new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc), item.CreationTime);
    }

    [Fact]
    public async Task ListAllPagesUntilTheTotalIsReached()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Page(3, 1, 2)).Respond(200, Page(3, 3));
        var request = new ListTransactionsRequest { MaxResultCount = 2 };

        var all = new List<AccountTransaction>();
        await foreach (var item in t.Client.Account.ListAllTransactionsAsync(request))
        {
            all.Add(item);
        }

        Assert.Equal(new[] { 1, 2, 3 }, all.Select(i => i.Period));
        Assert.Equal("0", t.Handler.Requests[0].Query("SkipCount"));
        Assert.Equal("2", t.Handler.Requests[1].Query("SkipCount"));
        Assert.Equal("2", t.Handler.Requests[1].Query("MaxResultCount"));
        Assert.Null(request.SkipCount); // the caller's request is not changed
    }

    [Fact]
    public async Task ListAllStopsOnAnEmptyPage()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Page(10, 1)).Respond(200, Page(10));

        var count = 0;
        await foreach (var _ in t.Client.Account.ListAllTransactionsAsync())
        {
            count++;
        }

        Assert.Equal(1, count);
        Assert.Equal("100", t.Handler.Requests[0].Query("MaxResultCount"));
    }

    [Fact]
    public async Task RejectsANegativeSkipCount()
    {
        using var t = new TestClient();

        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Account.ListTransactionsAsync(new ListTransactionsRequest { SkipCount = -1 }));
    }
}
