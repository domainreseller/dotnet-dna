namespace DomainNameApi.Tests.Infrastructure;

/// <summary>Creates clients wired to a <see cref="FakeHttpHandler"/>, with pacing off and waits recorded instead of slept.</summary>
internal sealed class TestClient : IDisposable
{
    public const string ResellerId = "00000000-0000-4000-8000-000000000001";
    public const string ApiKey = "test-api-key";

    public TestClient(Action<DomainNameApiOptions>? configure = null)
    {
        var options = new DomainNameApiOptions
        {
            ResellerId = ResellerId,
            ApiKey = ApiKey,
            Environment = ApiEnvironment.Ote,
            PaceRequests = false,
        };
        configure?.Invoke(options);
        Handler = new FakeHttpHandler();
        Client = new DomainNameApiClient(new HttpClient(Handler), options)
        {
            Delay = (wait, _) =>
            {
                Delays.Add(wait);
                return Task.CompletedTask;
            },
        };
    }

    public DomainNameApiClient Client { get; }

    public FakeHttpHandler Handler { get; }

    /// <summary>Every wait the client asked for (retries and pacing).</summary>
    public List<TimeSpan> Delays { get; } = [];

    public RecordedRequest LastRequest => Handler.LastRequest;

    public void Dispose() => Client.Dispose();
}
