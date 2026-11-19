using Microsoft.Extensions.DependencyInjection;

namespace DomainNameApi.Tests.Core;

public class DependencyInjectionTests
{
    [Fact]
    public async Task WorksAsATypedHttpClient()
    {
        var handler = new FakeHttpHandler().Respond(200, """{"resellerName":"Acme"}""");
        var services = new ServiceCollection();
        services.AddSingleton(new DomainNameApiOptions
        {
            ResellerId = TestClient.ResellerId,
            ApiKey = TestClient.ApiKey,
            Environment = ApiEnvironment.Ote,
        });
        services.AddHttpClient<DomainNameApiClient>().ConfigurePrimaryHttpMessageHandler(() => handler);

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<DomainNameApiClient>();
        var account = await client.Account.GetAsync();

        Assert.Equal("Acme", account.ResellerName);
        Assert.Equal(TestClient.ApiKey, handler.LastRequest.Headers["X-API-KEY"]);
    }
}
