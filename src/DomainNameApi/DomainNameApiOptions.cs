namespace DomainNameApi;

/// <summary>
/// Settings of a <see cref="DomainNameApiClient"/>. Only <see cref="ResellerId"/> and <see cref="ApiKey"/>
/// are required; everything else has a sensible default.
/// </summary>
public sealed class DomainNameApiOptions
{
    /// <summary>Your reseller ID (a UUID), from the reseller panel's API settings. Sent as the <c>__reseller</c> header.</summary>
    public string ResellerId { get; set; } = "";

    /// <summary>
    /// Your API key, from the reseller panel's API settings. Sent as the <c>X-API-KEY</c> header.
    /// Production and OTE use different keys.
    /// </summary>
    public string ApiKey { get; set; } = "";

    /// <summary>Which deployment to use. <see cref="ApiEnvironment.Production"/> by default.</summary>
    public ApiEnvironment Environment { get; set; } = ApiEnvironment.Production;

    /// <summary>
    /// Overrides the base address chosen by <see cref="Environment"/>, for example for a proxy or a
    /// local test server. Only the scheme, host, port and path prefix are used.
    /// </summary>
    public Uri? BaseAddress { get; set; }

    /// <summary>Time limit of one attempt. 60 seconds by default.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// How often a failed request is repeated. 2 by default; 0 turns retries off.
    /// 429 answers are always retried (the gateway refused them before anything ran).
    /// Network errors and 502/503/504 answers are retried only for reads, so nothing is charged twice.
    /// </summary>
    public int MaxRetries { get; set; } = 2;

    /// <summary>The longest wait before a retry. A <c>Retry-After</c> longer than this is not waited for. 30 seconds by default.</summary>
    public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Spaces requests to stay under the gateway's per-key rate limits: 5 requests/second in general,
    /// 2/second for domain searches and 1/second for DNS zone calls. On by default.
    /// </summary>
    public bool PaceRequests { get; set; } = true;

    /// <summary>
    /// Blocks every request that is not a read (<see cref="OperationSafety.Read"/>) before it is sent.
    /// Useful while exploring the API or in reporting tools.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// The <c>User-Agent</c> header. Defaults to <c>DomainNameAPI-DotNet/&lt;version&gt;</c>.
    /// The API sits behind Cloudflare, which rejects empty or generic agents.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// The application that uses the SDK, for example <c>whmcs</c> or <c>my-panel</c>. Sent as the <c>X-App</c> header on
    /// every request, lowercased and reduced to <c>a-z</c>, <c>0-9</c>, <c>.</c>, <c>_</c> and <c>-</c>.
    /// Defaults to <c>dotnet</c>; an empty value, or one with none of those characters, also sends <c>dotnet</c>.
    /// </summary>
    public string? AppName { get; set; }

    /// <summary>
    /// The version of the application that uses the SDK, for example <c>2.0.20</c>. Sent as the <c>X-Version</c> header on
    /// every request with only digits and dots kept (<c>V2.0.20-beta</c> is sent as <c>2.0.20</c>).
    /// Defaults to the SDK version (<see cref="DomainNameApiClient.Version"/>), which is also sent when this is empty.
    /// </summary>
    public string? AppVersion { get; set; }

    /// <summary>
    /// Reads the settings the DomainNameAPI tools share:
    /// <list type="bullet">
    /// <item><c>DNA_RESELLER_ID</c> and <c>DNA_API_KEY</c>: the credentials.</item>
    /// <item><c>DNA_ENV</c> (<c>production</c> or <c>ote</c>) or <c>DNA_TEST_MODE</c> (<c>true</c> for OTE): the environment.</item>
    /// <item><c>DNA_BASE_URL</c>: an optional base address override.</item>
    /// <item><c>DNA_READ_ONLY</c> (<c>true</c>): block everything but reads.</item>
    /// <item><c>DNA_APP</c> and <c>DNA_APP_VERSION</c>: the <see cref="AppName"/> and <see cref="AppVersion"/> sent as
    /// <c>X-App</c> and <c>X-Version</c>.</item>
    /// </list>
    /// </summary>
    /// <exception cref="ArgumentException">DNA_ENV or DNA_BASE_URL has a value that cannot be read.</exception>
    public static DomainNameApiOptions FromEnvironment()
    {
        var options = new DomainNameApiOptions
        {
            ResellerId = Variable("DNA_RESELLER_ID"),
            ApiKey = Variable("DNA_API_KEY"),
        };

        var env = Variable("DNA_ENV");
        if (env.Length == 0)
        {
            env = Variable("DNA_TEST_MODE");
        }
        if (!ApiEnvironments.TryParse(env, out var environment))
        {
            throw new ArgumentException("DNA_ENV must be production or ote, not '" + env + "'.");
        }
        options.Environment = environment;

        var baseUrl = Variable("DNA_BASE_URL");
        if (baseUrl.Length > 0)
        {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException("DNA_BASE_URL is not an absolute URL: '" + baseUrl + "'.");
            }
            options.BaseAddress = uri;
        }

        options.ReadOnly = Variable("DNA_READ_ONLY").ToLowerInvariant() is "true" or "1" or "on" or "yes";

        var app = Variable("DNA_APP");
        options.AppName = app.Length > 0 ? app : null;
        var appVersion = Variable("DNA_APP_VERSION");
        options.AppVersion = appVersion.Length > 0 ? appVersion : null;
        return options;
    }

    /// <summary>Returns a copy, so later changes to this object do not affect a client that already uses it.</summary>
    public DomainNameApiOptions Clone() => (DomainNameApiOptions)MemberwiseClone();

    /// <summary>The base address requests are sent to: <see cref="BaseAddress"/> or the environment's address.</summary>
    public Uri ResolveBaseAddress()
    {
        var address = BaseAddress ?? Environment.BaseAddress();
        var text = address.GetLeftPart(UriPartial.Path);
        return new Uri(text.EndsWith("/", StringComparison.Ordinal) ? text : text + "/");
    }

    private static string Variable(string name) => (System.Environment.GetEnvironmentVariable(name) ?? "").Trim();
}
