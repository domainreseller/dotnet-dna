namespace DomainNameApi.Tests.Infrastructure;

/// <summary>A request the fake handler received.</summary>
internal sealed class RecordedRequest
{
    public RecordedRequest(string method, Uri uri, IReadOnlyDictionary<string, string> headers, string? body)
    {
        Method = method;
        Uri = uri;
        Headers = headers;
        Body = body;
    }

    public string Method { get; }

    public Uri Uri { get; }

    public IReadOnlyDictionary<string, string> Headers { get; }

    public string? Body { get; }

    /// <summary>The path below /api/v1/, for example <c>domains/info</c>.</summary>
    public string ApiPath
    {
        get
        {
            const string prefix = "/api/v1/";
            var path = Uri.AbsolutePath;
            return path.StartsWith(prefix, StringComparison.Ordinal) ? path.Substring(prefix.Length) : path;
        }
    }

    /// <summary>The decoded query parameters, in order.</summary>
    public IReadOnlyList<KeyValuePair<string, string>> QueryParameters =>
        Uri.Query.TrimStart('?')
            .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(pair =>
            {
                var eq = pair.IndexOf('=');
                var name = eq < 0 ? pair : pair.Substring(0, eq);
                var value = eq < 0 ? "" : pair.Substring(eq + 1);
                return new KeyValuePair<string, string>(Uri.UnescapeDataString(name), Uri.UnescapeDataString(value));
            })
            .ToList();

    /// <summary>The first value of a query parameter, or null.</summary>
    public string? Query(string name) => QueryParameters.Where(p => p.Key == name).Select(p => p.Value).FirstOrDefault();

    /// <summary>Every value of a repeated query parameter.</summary>
    public IReadOnlyList<string> QueryAll(string name) => QueryParameters.Where(p => p.Key == name).Select(p => p.Value).ToList();

    /// <summary>The JSON body.</summary>
    public JsonElement Json => JsonDocument.Parse(Body ?? throw new InvalidOperationException("The request has no body.")).RootElement;

    public override string ToString() => Method + " " + Uri;
}
