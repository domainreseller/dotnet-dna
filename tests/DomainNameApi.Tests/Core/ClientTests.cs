using DomainNameApi.Accounts;
using DomainNameApi.Domains;

namespace DomainNameApi.Tests.Core;

public class ClientTests
{
    private const string Account = """{"resellerId":"00000000-0000-4000-8000-000000000001","resellerName":"Acme","resellerGroupName":"Gold","tryBalance":1500.5,"usdBalance":42.25}""";

    [Fact]
    public async Task SendsCredentialsAndStandardHeaders()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Account);

        await t.Client.Account.GetAsync();

        var request = t.LastRequest;
        Assert.Equal("GET", request.Method);
        Assert.Equal("https://ote.domainresellerapi.com/api/v1/deposit/accounts/me", request.Uri.ToString());
        Assert.Equal(TestClient.ApiKey, request.Headers["X-API-KEY"]);
        Assert.Equal(TestClient.ResellerId, request.Headers["__reseller"]);
        Assert.Equal("application/json", request.Headers["Accept"]);
        Assert.StartsWith("DomainNameAPI-DotNet/" + DomainNameApiClient.Version, request.Headers["User-Agent"]);
        Assert.Null(request.Body);
    }

    [Fact]
    public async Task UsesTheProductionAddressByDefault()
    {
        using var t = new TestClient(o => o.Environment = ApiEnvironment.Production);
        t.Handler.Respond(200, Account);

        await t.Client.Account.GetAsync();

        Assert.Equal("api.domainresellerapi.com", t.LastRequest.Uri.Host);
        Assert.Equal(ApiEnvironment.Production, t.Client.Environment);
    }

    [Fact]
    public async Task BaseAddressOverrideKeepsItsPathPrefix()
    {
        using var t = new TestClient(o => o.BaseAddress = new Uri("http://localhost:8080/proxy"));
        t.Handler.Respond(200, Account);

        await t.Client.Account.GetAsync();

        Assert.Equal("http://localhost:8080/proxy/api/v1/deposit/accounts/me", t.LastRequest.Uri.ToString());
    }

    [Fact]
    public async Task CustomUserAgentReplacesTheDefault()
    {
        using var t = new TestClient(o => o.UserAgent = "MyPanel/3.1");
        t.Handler.Respond(200, Account);

        await t.Client.Account.GetAsync();

        Assert.Equal("MyPanel/3.1", t.LastRequest.Headers["User-Agent"]);
    }

    [Fact]
    public async Task CredentialsAreTrimmed()
    {
        using var t = new TestClient(o =>
        {
            o.ApiKey = "  key-with-spaces \n";
            o.ResellerId = " " + TestClient.ResellerId + " ";
        });
        t.Handler.Respond(200, Account);

        await t.Client.Account.GetAsync();

        Assert.Equal("key-with-spaces", t.LastRequest.Headers["X-API-KEY"]);
        Assert.Equal(TestClient.ResellerId, t.LastRequest.Headers["__reseller"]);
    }

    [Fact]
    public async Task DecodesTheResponse()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Account);

        var account = await t.Client.Account.GetAsync();

        Assert.Equal(Guid.Parse(TestClient.ResellerId), account.ResellerId);
        Assert.Equal("Acme", account.ResellerName);
        Assert.Equal(1500.5m, account.TryBalance);
        Assert.Equal(42.25m, account.UsdBalance);
    }

    [Fact]
    public async Task MissingCredentialsFailBeforeSending()
    {
        using var t = new TestClient(o => o.ApiKey = " ");

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Equal("NO_CREDENTIALS", e.Code);
        Assert.Equal(ApiErrorKind.Authentication, e.Kind);
        Assert.NotNull(e.Hint);
        Assert.Empty(t.Handler.Requests);
    }

    [Fact]
    public async Task InvalidArgumentsFailBeforeSending()
    {
        using var t = new TestClient();

        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Domains.GetAsync(" "));
        await Assert.ThrowsAsync<ArgumentException>(() => t.Client.Domains.RenewAsync("example.com", years: 0));

        Assert.Empty(t.Handler.Requests);
    }

    [Fact]
    public async Task ReadOnlyModeBlocksChangesButAllowsReads()
    {
        using var t = new TestClient(o => o.ReadOnly = true);
        t.Handler.Respond(200, Account);

        await t.Client.Account.GetAsync();
        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Domains.RenewAsync("example.com"));

        Assert.Equal(ApiErrorKind.ReadOnly, e.Kind);
        Assert.Equal("READ_ONLY", e.Code);
        Assert.Equal(OperationSafety.Billable, e.Safety);
        Assert.Single(t.Handler.Requests);
    }

    [Fact]
    public async Task OptionsAreCopiedAtConstruction()
    {
        var options = new DomainNameApiOptions { ResellerId = TestClient.ResellerId, ApiKey = "first", PaceRequests = false };
        var handler = new FakeHttpHandler().Respond(200, Account);
        using var client = new DomainNameApiClient(new HttpClient(handler), options);

        options.ApiKey = "second";
        options.ReadOnly = true;
        await client.Account.GetAsync();

        Assert.Equal("first", handler.LastRequest.Headers["X-API-KEY"]);
        Assert.Equal("first", client.Options.ApiKey);
    }

    [Fact]
    public void RejectsInvalidOptions()
    {
        Assert.Throws<ArgumentNullException>(() => new DomainNameApiClient((DomainNameApiOptions)null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DomainNameApiClient(new DomainNameApiOptions { Timeout = TimeSpan.Zero }));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DomainNameApiClient(new DomainNameApiOptions { MaxRetries = -1 }));
    }

    [Fact]
    public async Task SendWithResponseExposesCorrelationIdQuotaAndAttempts()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Account,
            ("X-Correlation-Id", "abc123"),
            ("X-Quota-Scope", "deposit"),
            ("X-Quota-Cost", "1"),
            ("X-Quota-Used", "205"),
            ("X-Quota-Limit", "200"),
            ("X-Quota-Delay", "1.5"));

        var response = await t.Client.SendWithResponseAsync(new GetAccountRequest());

        Assert.Equal(200, response.StatusCode);
        Assert.Equal("abc123", response.CorrelationId);
        Assert.Equal(1, response.Attempts);
        Assert.Equal("Acme", response.Value.ResellerName);
        Assert.NotNull(response.Quota);
        Assert.Equal("deposit", response.Quota!.Scope);
        Assert.Equal(205, response.Quota.Used);
        Assert.True(response.Quota.WasDelayed);
    }

    [Fact]
    public async Task NoQuotaWhenTheGatewaySendsNone()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Account);

        var response = await t.Client.SendWithResponseAsync(new GetAccountRequest());

        Assert.Null(response.Quota);
        Assert.Null(response.CorrelationId);
    }

    [Fact]
    public async Task CallerCancellationIsNotTurnedIntoAnApiError()
    {
        using var t = new TestClient();
        t.Handler.Hang();
        using var cts = new CancellationTokenSource();

        var task = t.Client.Account.GetAsync(cts.Token);
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
    }

    [Fact]
    public async Task DisposedClientRefusesRequests()
    {
        var t = new TestClient();
        t.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => t.Client.Account.GetAsync());
    }

    [Fact]
    public void VersionComesFromTheAssembly()
    {
        Assert.Matches(@"^\d+\.\d+\.\d+", DomainNameApiClient.Version);
        Assert.Contains(DomainNameApiClient.Version, DomainNameApiClient.DefaultUserAgent);
    }

    [Fact]
    public async Task RequestMetadataIsNotWrittenToTheBody()
    {
        using var t = new TestClient();
        t.Handler.Respond(200);

        await t.Client.Domains.LockAsync("example.com");

        var body = t.LastRequest.Json;
        Assert.Equal(new[] { "domainName" }, body.Names());
        Assert.Equal("example.com", body.Str("domainName"));
    }

    [Fact]
    public async Task NullPropertiesAreLeftOutOfTheBody()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"success":true}""");

        await t.Client.Domains.SearchAsync("example.com");

        Assert.False(t.LastRequest.Json.Has("currency"));
    }

    [Fact]
    public async Task SendAsyncAcceptsCustomRequests()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, """{"notes":["first"]}""");

        var result = await t.Client.SendAsync(new GetNotesRequest("example.com"));

        Assert.Equal("domains/notes", t.LastRequest.ApiPath);
        Assert.Equal("example.com", t.LastRequest.Query("DomainName"));
        Assert.Equal(new[] { "first" }, result.Notes);
    }

    private sealed class NotesResult
    {
        public List<string> Notes { get; set; } = [];
    }

    // The example from the BaseRequest documentation: a request for an endpoint the SDK does not cover.
    private sealed class GetNotesRequest : BaseRequest<NotesResult>
    {
        public GetNotesRequest(string domainName) => DomainName = domainName;

        public string DomainName { get; }

        public override HttpMethod Method => HttpMethod.Get;

        public override string Path => "domains/notes";

        public override OperationSafety Safety => OperationSafety.Read;

        // "protected internal" only because this test assembly sees the SDK internals; your own requests use "protected".
        protected internal override void AddQueryParameters(QueryParameters query) => query.Add("DomainName", DomainName);
    }
}
