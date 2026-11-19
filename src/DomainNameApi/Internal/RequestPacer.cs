namespace DomainNameApi.Internal;

/// <summary>
/// Spaces requests per rate-limit class so a client stays under the gateway's per-key limits:
/// 5 requests/second in general, 2/second for domain searches (which also count towards the
/// general budget) and a separate budget for DNS zone calls, which are kept 1 second apart.
/// Domain info and TLD list calls are not rate limited.
/// </summary>
internal sealed class RequestPacer
{
    private static readonly TimeSpan GeneralGap = TimeSpan.FromMilliseconds(200);
    private static readonly TimeSpan SearchGap = TimeSpan.FromMilliseconds(500);
    private static readonly TimeSpan ZoneGap = TimeSpan.FromSeconds(1);

    private readonly object _lock = new();
    private readonly Dictionary<string, DateTimeOffset> _next = new(StringComparer.Ordinal);
    private readonly Func<DateTimeOffset> _now;
    private readonly Func<TimeSpan, CancellationToken, Task> _delay;

    public RequestPacer(Func<DateTimeOffset> now, Func<TimeSpan, CancellationToken, Task> delay)
    {
        _now = now;
        _delay = delay;
    }

    /// <summary>Returns the rate-limit class of a path below /api/v1/ and the minimum gap between its requests.</summary>
    internal static (string Class, TimeSpan Gap) Classify(string path)
    {
        var p = path.TrimStart('/').ToLowerInvariant();
        var q = p.IndexOf('?');
        if (q >= 0)
        {
            p = p.Substring(0, q);
        }
        if (p.StartsWith("domains/zones", StringComparison.Ordinal))
        {
            return ("zones", ZoneGap);
        }
        if (p is "domains/search" or "domains/bulk-search")
        {
            return ("search", SearchGap);
        }
        if (p.StartsWith("domains/info", StringComparison.Ordinal) || p.StartsWith("products/tlds", StringComparison.Ordinal))
        {
            return ("", TimeSpan.Zero);
        }
        return ("general", GeneralGap);
    }

    /// <summary>Waits until a request to <paramref name="path"/> may be sent.</summary>
    public Task WaitAsync(string path, CancellationToken cancellationToken)
    {
        var (cls, gap) = Classify(path);
        if (gap == TimeSpan.Zero)
        {
            return Task.CompletedTask;
        }

        DateTimeOffset slot;
        DateTimeOffset now;
        lock (_lock)
        {
            now = _now();
            _next.TryGetValue(cls, out var next);
            slot = next > now ? next : now;
            _next[cls] = slot + gap;
            if (cls == "search")
            {
                // Searches also count towards the general budget; zone calls have their own.
                _next.TryGetValue("general", out var general);
                if (general < slot + GeneralGap)
                {
                    _next["general"] = slot + GeneralGap;
                }
            }
        }

        var wait = slot - now;
        return wait > TimeSpan.Zero ? _delay(wait, cancellationToken) : Task.CompletedTask;
    }
}
