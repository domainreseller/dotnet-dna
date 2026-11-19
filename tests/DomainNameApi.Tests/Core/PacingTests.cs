using DomainNameApi.Dns;
using DomainNameApi.Internal;

namespace DomainNameApi.Tests.Core;

public class PacingTests
{
    [Theory]
    [InlineData("domains/zones", "zones", 1000)]
    [InlineData("/domains/zones/apply", "zones", 1000)]
    [InlineData("domains/search", "search", 500)]
    [InlineData("domains/bulk-search", "search", 500)]
    [InlineData("domains/info", "", 0)]
    [InlineData("products/tlds", "", 0)]
    [InlineData("domains/renew", "general", 200)]
    [InlineData("deposit/accounts/me", "general", 200)]
    public void ClassifiesPaths(string path, string expectedClass, int gapMs)
    {
        var (cls, gap) = RequestPacer.Classify(path);

        Assert.Equal(expectedClass, cls);
        Assert.Equal(TimeSpan.FromMilliseconds(gapMs), gap);
    }

    [Fact]
    public async Task SpacesCallsOfTheSameClass()
    {
        var clock = new FakeClock();
        var pacer = new RequestPacer(() => clock.Now, clock.Delay);

        await pacer.WaitAsync("domains/zones", default);
        await pacer.WaitAsync("domains/zones", default);
        await pacer.WaitAsync("domains/renew", default);   // the general budget is separate from zones
        await pacer.WaitAsync("domains/info", default);    // not rate limited

        Assert.Equal(new[] { TimeSpan.FromSeconds(1) }, clock.Waits);
    }

    [Fact]
    public async Task SearchesAlsoUseTheGeneralBudget()
    {
        var clock = new FakeClock();
        var pacer = new RequestPacer(() => clock.Now, clock.Delay);

        await pacer.WaitAsync("domains/search", default);
        await pacer.WaitAsync("domains/renew", default);

        Assert.Equal(new[] { TimeSpan.FromMilliseconds(200) }, clock.Waits);
    }

    [Fact]
    public async Task DoesNotWaitWhenEnoughTimePassed()
    {
        var clock = new FakeClock();
        var pacer = new RequestPacer(() => clock.Now, clock.Delay);

        await pacer.WaitAsync("domains/zones", default);
        clock.Now += TimeSpan.FromSeconds(3);
        await pacer.WaitAsync("domains/zones", default);

        Assert.Empty(clock.Waits);
    }

    [Fact]
    public async Task TheClientPacesZoneCalls()
    {
        using var t = new TestClient(o => o.PaceRequests = true);
        var now = DateTimeOffset.UtcNow;
        t.Client.Now = () => now;
        t.Handler.Respond(200, "[]").Respond(200, "[]");

        await t.Client.Dns.ListAsync("example.com");
        await t.Client.Dns.ListAsync("example.com");

        Assert.Equal(new[] { TimeSpan.FromSeconds(1) }, t.Delays);
    }

    private sealed class FakeClock
    {
        public DateTimeOffset Now { get; set; } = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public List<TimeSpan> Waits { get; } = [];

        public Task Delay(TimeSpan wait, CancellationToken cancellationToken)
        {
            Waits.Add(wait);
            Now += wait;
            return Task.CompletedTask;
        }
    }
}
