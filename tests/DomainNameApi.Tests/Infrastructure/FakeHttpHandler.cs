using System.Text;

namespace DomainNameApi.Tests.Infrastructure;

/// <summary>
/// Stands in for the network: records every request and answers with queued responses.
/// No test talks to the real API.
/// </summary>
internal sealed class FakeHttpHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>> _responses = new();

    public List<RecordedRequest> Requests { get; } = [];

    public RecordedRequest LastRequest => Requests.Count > 0 ? Requests[Requests.Count - 1] : throw new InvalidOperationException("No request was sent.");

    /// <summary>Queues a response with a JSON (or any text) body.</summary>
    public FakeHttpHandler Respond(int status, string? body = null, params (string Name, string Value)[] headers) =>
        Enqueue((request, _) =>
        {
            var response = new HttpResponseMessage((HttpStatusCode)status)
            {
                Content = new StringContent(body ?? "", Encoding.UTF8, "application/json"),
            };
            foreach (var (name, value) in headers)
            {
                if (!response.Headers.TryAddWithoutValidation(name, value))
                {
                    response.Content.Headers.TryAddWithoutValidation(name, value);
                }
            }
            return Task.FromResult(response);
        });

    /// <summary>Queues a 200 response with the object written as JSON.</summary>
    public FakeHttpHandler RespondJson(object value) => Respond(200, JsonSerializer.Serialize(value));

    /// <summary>Queues an exception, as if the network failed.</summary>
    public FakeHttpHandler Fail(Exception exception) => Enqueue((_, _) => Task.FromException<HttpResponseMessage>(exception));

    /// <summary>Queues a response that never comes (until the request is cancelled).</summary>
    public FakeHttpHandler Hang() => Enqueue(async (_, ct) =>
    {
        await Task.Delay(Timeout.Infinite, ct);
        throw new InvalidOperationException("unreachable");
    });

    public FakeHttpHandler Enqueue(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
    {
        _responses.Enqueue(responder);
        return this;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null ? null : await request.Content.ReadAsStringAsync();
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in request.Headers)
        {
            headers[header.Key] = string.Join(", ", header.Value);
        }
        if (request.Content is not null)
        {
            foreach (var header in request.Content.Headers)
            {
                headers[header.Key] = string.Join(", ", header.Value);
            }
        }
        Requests.Add(new RecordedRequest(request.Method.Method, request.RequestUri!, headers, body));

        if (_responses.Count == 0)
        {
            throw new InvalidOperationException("No response queued for " + request.Method + " " + request.RequestUri);
        }
        var response = await _responses.Dequeue()(request, cancellationToken);
        response.RequestMessage ??= request;
        return response;
    }
}
