namespace DomainNameApi.Tests.Core;

[Collection(EnvironmentScope.Collection)]
public class ClientInfoTests
{
    private const string Account = """{"resellerId":"00000000-0000-4000-8000-000000000001","resellerName":"Acme","resellerGroupName":"Gold","tryBalance":1500.5,"usdBalance":42.25}""";

    [Fact]
    public async Task SendsDotnetAndTheSdkVersionByDefault()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, Account);

        await t.Client.Account.GetAsync();

        Assert.Equal("dotnet", t.LastRequest.Headers["X-App"]);
        Assert.Matches(@"^\d+(\.\d+)*$", t.LastRequest.Headers["X-Version"]);
        Assert.StartsWith(t.LastRequest.Headers["X-Version"], DomainNameApiClient.Version);
        Assert.Equal(DomainNameApiClient.DefaultAppName, t.Client.AppName);
        Assert.Equal(t.LastRequest.Headers["X-Version"], t.Client.AppVersion);
    }

    [Fact]
    public async Task SendsTheConfiguredApplication()
    {
        using var t = new TestClient(o =>
        {
            o.AppName = "whmcs";
            o.AppVersion = "8.13.1";
        });
        t.Handler.Respond(200, Account);

        await t.Client.Account.GetAsync();

        Assert.Equal("whmcs", t.LastRequest.Headers["X-App"]);
        Assert.Equal("8.13.1", t.LastRequest.Headers["X-Version"]);
    }

    [Theory]
    [InlineData("WiseCP5", "V2.0.20-beta", "wisecp5", "2.0.20")]
    [InlineData("  My Panel! ", ".1.2.", "mypanel", "1.2")]
    [InlineData("host_bill-2.x", "v10", "host_bill-2.x", "10")]
    public async Task NormalizesTheValues(string app, string version, string expectedApp, string expectedVersion)
    {
        using var t = new TestClient(o =>
        {
            o.AppName = app;
            o.AppVersion = version;
        });
        t.Handler.Respond(200, Account);

        await t.Client.Account.GetAsync();

        Assert.Equal(expectedApp, t.LastRequest.Headers["X-App"]);
        Assert.Equal(expectedVersion, t.LastRequest.Headers["X-Version"]);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("   ", "beta")]
    [InlineData("!!!", "...")]
    public void EmptyOrInvalidValuesFallBackToTheDefaults(string? app, string? version)
    {
        using var t = new TestClient(o =>
        {
            o.AppName = app;
            o.AppVersion = version;
        });

        using var defaults = new TestClient();
        Assert.Equal("dotnet", t.Client.AppName);
        Assert.Equal(defaults.Client.AppVersion, t.Client.AppVersion);
        Assert.Matches(@"^\d+(\.\d+)*$", t.Client.AppVersion);
    }

    [Fact]
    public async Task RetriesCarryTheHeadersToo()
    {
        using var t = new TestClient(o =>
        {
            o.AppName = "wisecp";
            o.AppVersion = "2.0.20";
        });
        t.Handler.Respond(503, "").Respond(200, Account);

        await t.Client.Account.GetAsync();

        Assert.Equal(2, t.Handler.Requests.Count);
        Assert.All(t.Handler.Requests, request =>
        {
            Assert.Equal("wisecp", request.Headers["X-App"]);
            Assert.Equal("2.0.20", request.Headers["X-Version"]);
        });
    }

    [Fact]
    public async Task UserAgentIsUnchanged()
    {
        using var t = new TestClient(o => o.AppName = "whmcs");
        t.Handler.Respond(200, Account);

        await t.Client.Account.GetAsync();

        Assert.StartsWith("DomainNameAPI-DotNet/" + DomainNameApiClient.Version, t.LastRequest.Headers["User-Agent"]);
        Assert.DoesNotContain("whmcs", t.LastRequest.Headers["User-Agent"]);
    }

    [Fact]
    public void ReadsTheApplicationFromTheEnvironment()
    {
        using var vars = new EnvironmentScope(("DNA_ENV", null), ("DNA_TEST_MODE", null), ("DNA_BASE_URL", null),
            ("DNA_APP", "WiseCP5"), ("DNA_APP_VERSION", "V2.0.20-beta"));

        var options = DomainNameApiOptions.FromEnvironment();
        using var client = new DomainNameApiClient(options);

        Assert.Equal("WiseCP5", options.AppName);
        Assert.Equal("V2.0.20-beta", options.AppVersion);
        Assert.Equal("wisecp5", client.AppName);
        Assert.Equal("2.0.20", client.AppVersion);
    }

    [Fact]
    public void UnsetEnvironmentVariablesKeepTheDefaults()
    {
        using var vars = new EnvironmentScope(("DNA_ENV", null), ("DNA_TEST_MODE", null), ("DNA_BASE_URL", null),
            ("DNA_APP", null), ("DNA_APP_VERSION", "  "));

        var options = DomainNameApiOptions.FromEnvironment();
        using var client = new DomainNameApiClient(options);

        Assert.Null(options.AppName);
        Assert.Null(options.AppVersion);
        Assert.Equal("dotnet", client.AppName);
    }
}
