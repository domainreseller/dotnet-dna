namespace DomainNameApi.Tests.Core;

[Collection(EnvironmentScope.Collection)]
public class OptionsTests
{
    [Theory]
    [InlineData("", ApiEnvironment.Production)]
    [InlineData("production", ApiEnvironment.Production)]
    [InlineData("PROD", ApiEnvironment.Production)]
    [InlineData("false", ApiEnvironment.Production)]
    [InlineData("ote", ApiEnvironment.Ote)]
    [InlineData("Test", ApiEnvironment.Ote)]
    [InlineData("sandbox", ApiEnvironment.Ote)]
    [InlineData("true", ApiEnvironment.Ote)]
    [InlineData("1", ApiEnvironment.Ote)]
    public void ParsesEnvironmentNames(string text, ApiEnvironment expected)
    {
        Assert.True(ApiEnvironments.TryParse(text, out var environment));
        Assert.Equal(expected, environment);
    }

    [Fact]
    public void RejectsUnknownEnvironmentNames() => Assert.False(ApiEnvironments.TryParse("staging", out _));

    [Fact]
    public void ReadsTheSharedEnvironmentVariables()
    {
        using var vars = new EnvironmentScope(
            ("DNA_RESELLER_ID", TestClient.ResellerId),
            ("DNA_API_KEY", "from-env"),
            ("DNA_ENV", null),
            ("DNA_TEST_MODE", "true"),
            ("DNA_BASE_URL", "http://localhost:9000/"),
            ("DNA_READ_ONLY", "yes"));

        var options = DomainNameApiOptions.FromEnvironment();

        Assert.Equal(TestClient.ResellerId, options.ResellerId);
        Assert.Equal("from-env", options.ApiKey);
        Assert.Equal(ApiEnvironment.Ote, options.Environment);
        Assert.Equal(new Uri("http://localhost:9000/"), options.BaseAddress);
        Assert.True(options.ReadOnly);
    }

    [Fact]
    public void DnaEnvWinsOverTestMode()
    {
        using var vars = new EnvironmentScope(("DNA_ENV", "production"), ("DNA_TEST_MODE", "true"), ("DNA_BASE_URL", null), ("DNA_READ_ONLY", null));

        Assert.Equal(ApiEnvironment.Production, DomainNameApiOptions.FromEnvironment().Environment);
    }

    [Fact]
    public void RejectsAnUnreadableEnvironment()
    {
        using var vars = new EnvironmentScope(("DNA_ENV", "staging"));

        Assert.Throws<ArgumentException>(() => DomainNameApiOptions.FromEnvironment());
    }

    [Fact]
    public void ResolvesBaseAddresses()
    {
        Assert.Equal("https://ote.domainresellerapi.com/", new DomainNameApiOptions { Environment = ApiEnvironment.Ote }.ResolveBaseAddress().ToString());
        Assert.Equal("https://api.domainresellerapi.com/", new DomainNameApiOptions().ResolveBaseAddress().ToString());
        Assert.Equal("http://proxy/dna/", new DomainNameApiOptions { BaseAddress = new Uri("http://proxy/dna?x=1") }.ResolveBaseAddress().ToString());
    }
}

/// <summary>Tests that change environment variables run one at a time.</summary>
[CollectionDefinition(EnvironmentScope.Collection, DisableParallelization = true)]
public sealed class EnvironmentVariablesCollection
{
}

/// <summary>Sets environment variables for one test and restores them afterwards.</summary>
internal sealed class EnvironmentScope : IDisposable
{
    public const string Collection = "EnvironmentVariables";

    private readonly List<(string Name, string? Value)> _previous = [];

    public EnvironmentScope(params (string Name, string? Value)[] variables)
    {
        foreach (var (name, value) in variables)
        {
            _previous.Add((name, Environment.GetEnvironmentVariable(name)));
            Environment.SetEnvironmentVariable(name, value);
        }
    }

    public void Dispose()
    {
        foreach (var (name, value) in _previous)
        {
            Environment.SetEnvironmentVariable(name, value);
        }
    }
}
