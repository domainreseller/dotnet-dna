using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using DomainNameApi.Accounts;
using DomainNameApi.Contacts;
using DomainNameApi.Dns;
using DomainNameApi.Dnssec;
using DomainNameApi.Domains;
using DomainNameApi.Hosting;
using DomainNameApi.Internal;
using DomainNameApi.Orders;
using DomainNameApi.Products;
using DomainNameApi.Ssl;

namespace DomainNameApi;

/// <summary>
/// The entry point of the SDK. Each area of the API has its own client:
/// <see cref="Domains"/>, <see cref="Contacts"/>, <see cref="Dns"/>, <see cref="Dnssec"/>, <see cref="Ssl"/>,
/// <see cref="Hosting"/>, <see cref="Orders"/>, <see cref="Products"/> and <see cref="Account"/>.
/// Any request, including your own <see cref="BaseRequest"/> subclasses, can also be sent with
/// <see cref="SendAsync{TResponse}(BaseRequest{TResponse}, CancellationToken)"/>.
/// </summary>
/// <remarks>
/// <para>
/// The client is thread-safe and meant to be reused: create one per set of credentials and keep it for
/// the lifetime of the application (or register it with <c>AddHttpClient&lt;DomainNameApiClient&gt;()</c>).
/// </para>
/// <para>
/// Failed requests throw <see cref="DomainNameApiException"/>. Rate limits are respected automatically,
/// 429 answers are retried, and network errors are retried only for reads, so nothing is charged twice.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// using var client = new DomainNameApiClient(resellerId, apiKey, ApiEnvironment.Ote);
///
/// var account = await client.Account.GetAsync();
/// var search = await client.Domains.SearchAsync("example.com");
/// await foreach (var domain in client.Domains.ListAllAsync())
/// {
///     Console.WriteLine($"{domain.DomainName} expires {domain.ExpirationDate:d}");
/// }
/// </code>
/// </example>
public sealed class DomainNameApiClient : IDisposable
{
    private const string ApiPrefix = "api/v1/";
    private static readonly Random Jitter = new();

    private readonly HttpClient _http;
    private readonly bool _ownsHttpClient;
    private readonly DomainNameApiOptions _options;
    private readonly string _userAgent;
    private readonly RequestPacer _pacer;
    private bool _disposed;

    /// <summary>Creates a client for the given credentials.</summary>
    /// <param name="resellerId">Your reseller ID (a UUID) from the reseller panel's API settings.</param>
    /// <param name="apiKey">Your API key from the reseller panel's API settings.</param>
    /// <param name="environment">Production (default) or the OTE test environment.</param>
    public DomainNameApiClient(string resellerId, string apiKey, ApiEnvironment environment = ApiEnvironment.Production)
        : this(new DomainNameApiOptions { ResellerId = resellerId, ApiKey = apiKey, Environment = environment })
    {
    }

    /// <summary>Creates a client with its own <see cref="HttpClient"/>.</summary>
    /// <param name="options">The settings. They are copied; later changes have no effect.</param>
    public DomainNameApiClient(DomainNameApiOptions options)
        : this(CreateHttpClient(), options, ownsHttpClient: true)
    {
    }

    /// <summary>
    /// Creates a client on top of an existing <see cref="HttpClient"/>, for example one from
    /// <c>IHttpClientFactory</c>. The client does not dispose it. Requests use absolute URLs, so the
    /// <see cref="HttpClient.BaseAddress"/> is not needed.
    /// </summary>
    /// <param name="httpClient">The HTTP client to send requests with.</param>
    /// <param name="options">The settings. They are copied; later changes have no effect.</param>
    public DomainNameApiClient(HttpClient httpClient, DomainNameApiOptions options)
        : this(httpClient, options, ownsHttpClient: false)
    {
    }

    private DomainNameApiClient(HttpClient httpClient, DomainNameApiOptions options, bool ownsHttpClient)
    {
        _http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        if (options.Timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Timeout must be positive.");
        }
        if (options.MaxRetries < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "MaxRetries cannot be negative.");
        }
        if (options.MaxRetryDelay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "MaxRetryDelay cannot be negative.");
        }

        _ownsHttpClient = ownsHttpClient;
        _options = options.Clone();
        _userAgent = string.IsNullOrWhiteSpace(_options.UserAgent) ? DefaultUserAgent : _options.UserAgent!.Trim();
        BaseAddress = _options.ResolveBaseAddress();
        _pacer = new RequestPacer(() => Now(), (wait, ct) => Delay(wait, ct));

        Account = new AccountClient(this);
        Contacts = new ContactsClient(this);
        Domains = new DomainsClient(this);
        Dns = new DnsClient(this);
        Dnssec = new DnssecClient(this);
        Ssl = new SslClient(this);
        Hosting = new HostingClient(this);
        Orders = new OrdersClient(this);
        Products = new ProductsClient(this);
    }

    /// <summary>
    /// Creates a client from the <c>DNA_RESELLER_ID</c>, <c>DNA_API_KEY</c>, <c>DNA_ENV</c> (or <c>DNA_TEST_MODE</c>),
    /// <c>DNA_BASE_URL</c> and <c>DNA_READ_ONLY</c> environment variables, the same ones the <c>dna</c> CLI reads.
    /// </summary>
    public static DomainNameApiClient FromEnvironment() => new(DomainNameApiOptions.FromEnvironment());

    /// <summary>The SDK version, for example <c>2.0.0</c>.</summary>
    public static string Version { get; } = ReadVersion();

    /// <summary>The default <c>User-Agent</c> header.</summary>
    public static string DefaultUserAgent { get; } =
        "DomainNameAPI-DotNet/" + Version + " (+https://github.com/domainreseller/dotnet-dna)";

    /// <summary>A copy of the settings this client uses.</summary>
    public DomainNameApiOptions Options => _options.Clone();

    /// <summary>The environment this client talks to.</summary>
    public ApiEnvironment Environment => _options.Environment;

    /// <summary>The base address requests are sent to.</summary>
    public Uri BaseAddress { get; }

    /// <summary>Your account: balances, profile and the transaction ledger.</summary>
    public AccountClient Account { get; }

    /// <summary>Reusable contact handles (registrant, admin, tech, billing) and their verification.</summary>
    public ContactsClient Contacts { get; }

    /// <summary>Search, register, renew, transfer and manage domains.</summary>
    public DomainsClient Domains { get; }

    /// <summary>DNS zone records of domains that use the platform nameservers.</summary>
    public DnsClient Dns { get; }

    /// <summary>DNSSEC (DS records) of domains.</summary>
    public DnssecClient Dnssec { get; }

    /// <summary>SSL certificates: buy, configure, issue, list, renew, reissue and revoke.</summary>
    public SslClient Ssl { get; }

    /// <summary>Web hosting packages and services.</summary>
    public HostingClient Hosting { get; }

    /// <summary>Orders.</summary>
    public OrdersClient Orders { get; }

    /// <summary>The product catalogue and prices: TLDs, SSL certificates and other products.</summary>
    public ProductsClient Products { get; }

    // Test seams: replace the clock and the waiting used by pacing and retries.
    internal Func<TimeSpan, CancellationToken, Task> Delay { get; set; } = (wait, ct) => Task.Delay(wait, ct);

    internal Func<DateTimeOffset> Now { get; set; } = () => DateTimeOffset.UtcNow;

    /// <summary>Sends a request that returns no data.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <exception cref="DomainNameApiException">The API refused the request, or it failed on the network.</exception>
    /// <exception cref="ArgumentException">The request is missing a required value; nothing was sent.</exception>
    public async Task SendAsync(BaseRequest request, CancellationToken cancellationToken = default) =>
        await SendCoreAsync(request, cancellationToken).ConfigureAwait(false);

    /// <summary>Sends a request and returns the decoded answer.</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <typeparam name="TResponse">The type of the answer.</typeparam>
    /// <exception cref="DomainNameApiException">The API refused the request, or it failed on the network.</exception>
    /// <exception cref="ArgumentException">The request is missing a required value; nothing was sent.</exception>
    public async Task<TResponse> SendAsync<TResponse>(BaseRequest<TResponse> request, CancellationToken cancellationToken = default) =>
        (await SendWithResponseAsync(request, cancellationToken).ConfigureAwait(false)).Value;

    /// <summary>Sends a request that returns no data and returns the HTTP details (correlation ID, quota, timing).</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    public async Task<ApiResponse> SendWithResponseAsync(BaseRequest request, CancellationToken cancellationToken = default) =>
        (await SendCoreAsync(request, cancellationToken).ConfigureAwait(false)).Response;

    /// <summary>Sends a request and returns the decoded answer with the HTTP details (correlation ID, quota, timing).</summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <typeparam name="TResponse">The type of the answer.</typeparam>
    public async Task<ApiResponse<TResponse>> SendWithResponseAsync<TResponse>(BaseRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var (response, body) = await SendCoreAsync(request, cancellationToken).ConfigureAwait(false);
        return new ApiResponse<TResponse>(Decode<TResponse>(request, response, body), response);
    }

    /// <summary>
    /// Pages through a list request and yields every item. The request's own <see cref="PagedRequest{TItem}.SkipCount"/>
    /// is the starting point and its <see cref="PagedRequest{TItem}.MaxResultCount"/> the page size
    /// (<see cref="PagedRequest{TItem}.DefaultPageSize"/> when not set). The request object is not changed.
    /// </summary>
    /// <param name="request">The list request with its filters.</param>
    /// <param name="cancellationToken">Stops the enumeration.</param>
    /// <typeparam name="TItem">The type of the items.</typeparam>
    public async IAsyncEnumerable<TItem> ListAllAsync<TItem>(
        PagedRequest<TItem> request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        var pageSize = request.MaxResultCount ?? PagedRequest<TItem>.DefaultPageSize;
        var skip = request.SkipCount ?? 0;
        while (true)
        {
            var page = await SendAsync(request.ForPage(skip, pageSize), cancellationToken).ConfigureAwait(false);
            foreach (var item in page.Items)
            {
                yield return item;
            }
            skip += page.Items.Count;
            if (page.Items.Count == 0 || skip >= page.TotalCount)
            {
                yield break;
            }
        }
    }

    /// <summary>Disposes the <see cref="HttpClient"/> if this client created it.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        if (_ownsHttpClient)
        {
            _http.Dispose();
        }
    }

    private async Task<(ApiResponse Response, string Body)> SendCoreAsync(BaseRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(DomainNameApiClient));
        }

        request.Validate();

        if (_options.ReadOnly && request.Safety != OperationSafety.Read)
        {
            throw ErrorParser.Create(request, ApiErrorKind.ReadOnly, "READ_ONLY",
                request + " is a " + request.Safety.ToString().ToLowerInvariant() + " operation and the client is read-only; nothing was sent.",
                status: null, correlationId: null, body: null);
        }
        if (string.IsNullOrWhiteSpace(_options.ResellerId) || string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw ErrorParser.Create(request, ApiErrorKind.Authentication, "NO_CREDENTIALS",
                "The reseller ID and API key are required; nothing was sent.",
                status: null, correlationId: null, body: null);
        }

        var query = new QueryParameters();
        request.AddQueryParameters(query);
        var uri = BuildUri(request.Path, query);
        var body = request.GetBody();
        var json = body is null ? null : JsonSerializer.Serialize(body, body.GetType(), Json.Options);
        var stopwatch = Stopwatch.StartNew();

        for (var attempt = 1; ; attempt++)
        {
            if (_options.PaceRequests)
            {
                await _pacer.WaitAsync(request.Path, cancellationToken).ConfigureAwait(false);
            }

            using var message = BuildMessage(request.Method, uri, json);
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(_options.Timeout);

            HttpResponseMessage? response = null;
            string text;
            try
            {
                response = await _http.SendAsync(message, HttpCompletionOption.ResponseContentRead, timeout.Token).ConfigureAwait(false);
                text = await ReadBodyAsync(response, timeout.Token).ConfigureAwait(false);
            }
            catch (Exception e) when (IsNetworkFailure(e, cancellationToken, out var timedOut))
            {
                response?.Dispose();
                if (request.IsIdempotent && attempt <= _options.MaxRetries)
                {
                    await Delay(Backoff(attempt), cancellationToken).ConfigureAwait(false);
                    continue;
                }
                throw ErrorParser.Network(request, e, timedOut, _options.Timeout);
            }
            catch
            {
                response?.Dispose();
                throw;
            }

            using (response)
            {
                var status = (int)response.StatusCode;
                var headers = ReadHeaders(response);
                var redirected = RedirectedToErrorPage(uri, response);
                var apiResponse = new ApiResponse(status, headers, attempt, stopwatch.Elapsed);

                if (status is >= 200 and < 300 && !redirected)
                {
                    return (apiResponse, text);
                }

                var retry = status == 429 || (request.IsIdempotent && status is 502 or 503 or 504);
                if (retry && !redirected && attempt <= _options.MaxRetries)
                {
                    var retryAfter = ErrorParser.RetryAfter(headers);
                    var wait = retryAfter is null ? Backoff(attempt) : retryAfter.Value + TimeSpan.FromMilliseconds(50);
                    if (wait <= _options.MaxRetryDelay)
                    {
                        await Delay(wait, cancellationToken).ConfigureAwait(false);
                        continue;
                    }
                }

                throw ErrorParser.FromResponse(request, status, headers, text, redirected);
            }
        }
    }

    private static T Decode<T>(BaseRequest request, ApiResponse response, string body)
    {
        var trimmed = body.Trim();
        if (trimmed.Length == 0 || trimmed == "null")
        {
            throw ErrorParser.Create(request, ApiErrorKind.Unknown, "EMPTY_RESPONSE",
                "The API answered " + request + " with an empty body.",
                response.StatusCode, response.CorrelationId, body);
        }
        try
        {
            return JsonSerializer.Deserialize<T>(trimmed, Json.Options)!;
        }
        catch (JsonException e)
        {
            throw ErrorParser.Create(request, ApiErrorKind.Unknown, "INVALID_RESPONSE",
                "The answer to " + request + " could not be read as " + typeof(T).Name + ": " + e.Message,
                response.StatusCode, response.CorrelationId, body, inner: e);
        }
    }

    private Uri BuildUri(string path, QueryParameters query)
    {
        var relative = ApiPrefix + (path ?? "").TrimStart('/');
        if (query.Count > 0)
        {
            relative += "?" + query;
        }
        return new Uri(BaseAddress, relative);
    }

    private HttpRequestMessage BuildMessage(HttpMethod method, Uri uri, string? json)
    {
        var message = new HttpRequestMessage(method, uri);
        message.Headers.TryAddWithoutValidation("Accept", "application/json");
        message.Headers.TryAddWithoutValidation("User-Agent", _userAgent);
        message.Headers.TryAddWithoutValidation("X-API-KEY", _options.ApiKey.Trim());
        message.Headers.TryAddWithoutValidation("__reseller", _options.ResellerId.Trim());
        if (json is not null)
        {
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }
        return message;
    }

    private static async Task<string> ReadBodyAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
#if NET
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
        cancellationToken.ThrowIfCancellationRequested();
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
    }

    private static Dictionary<string, string> ReadHeaders(HttpResponseMessage response)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in response.Headers)
        {
            headers[header.Key] = string.Join(", ", header.Value);
        }
        foreach (var header in response.Content.Headers)
        {
            headers[header.Key] = string.Join(", ", header.Value);
        }
        return headers;
    }

    /// <summary>
    /// An injected HttpClient may follow redirects. The gateway redirects unknown or suspended resellers to
    /// /Error/Suspend, which then answers 200 with an HTML page; treat that as the redirect it was.
    /// </summary>
    private static bool RedirectedToErrorPage(Uri requested, HttpResponseMessage response)
    {
        var final = response.RequestMessage?.RequestUri;
        return final is not null &&
               final.AbsolutePath != requested.AbsolutePath &&
               final.AbsolutePath.IndexOf("/Error/", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool IsNetworkFailure(Exception e, CancellationToken userToken, out bool timedOut)
    {
        timedOut = false;
        if (e is OperationCanceledException)
        {
            if (userToken.IsCancellationRequested)
            {
                return false; // the caller cancelled: let the OperationCanceledException through
            }
            timedOut = true;
            return true;
        }
        return e is HttpRequestException or IOException;
    }

    private static TimeSpan Backoff(int attempt)
    {
        int jitter;
        lock (Jitter)
        {
            jitter = Jitter.Next(250);
        }
        return TimeSpan.FromMilliseconds((500 << Math.Min(attempt - 1, 6)) + jitter);
    }

    private static HttpClient CreateHttpClient()
    {
#if NET
        var handler = new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.All,
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        };
#else
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        };
#endif
        // Each attempt has its own time limit (DomainNameApiOptions.Timeout).
        return new HttpClient(handler) { Timeout = System.Threading.Timeout.InfiniteTimeSpan };
    }

    private static string ReadVersion()
    {
        var assembly = typeof(DomainNameApiClient).Assembly;
        var informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (string.IsNullOrEmpty(informational))
        {
            return assembly.GetName().Version?.ToString(3) ?? "0.0.0";
        }
        var plus = informational!.IndexOf('+');
        return plus >= 0 ? informational.Substring(0, plus) : informational;
    }
}
