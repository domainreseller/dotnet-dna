using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DomainNameApi
{
    /// <summary>One HTTP request to the gateway, as built by the client.</summary>
    public sealed class HttpRequestData
    {
        /// <summary>GET, POST, PUT or DELETE.</summary>
        public string Method { get; set; }

        /// <summary>Absolute URL including the query string.</summary>
        public string Url { get; set; }

        /// <summary>Request headers (Content-Type, Accept, X-API-KEY, __reseller, User-Agent).</summary>
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>JSON body for POST/PUT, <c>null</c> for GET/DELETE.</summary>
        public string Body { get; set; }

        /// <summary>Total timeout.</summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>Connect timeout.</summary>
        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>Verify the server's TLS certificate.</summary>
        public bool VerifySsl { get; set; } = true;
    }

    /// <summary>The gateway's answer: status code, raw body and headers.</summary>
    public sealed class HttpResponseData
    {
        public int StatusCode { get; set; }

        /// <summary>Raw response text (may be empty, HTML, or JSON).</summary>
        public string Body { get; set; } = "";

        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string ContentType { get; set; } = "";

        /// <summary>Elapsed time of the request.</summary>
        public TimeSpan Elapsed { get; set; }
    }

    /// <summary>
    /// A transport-level failure (timeout, DNS, TLS, connection reset). The client
    /// turns it into an error envelope; it never escapes a public method.
    /// <see cref="ErrorNumber"/> follows libcurl's numbering so error codes match
    /// php-dna (28 = timeout, 6 = DNS, 7 = connect, 35/60 = TLS, 56 = receive).
    /// </summary>
    public sealed class TransportException : Exception
    {
        public TransportException(int errorNumber, string message, Exception inner = null) : base(message, inner)
        {
            ErrorNumber = errorNumber;
        }

        public int ErrorNumber { get; }

        public bool IsTimeout => ErrorNumber == 28;
    }

    /// <summary>
    /// Replaceable HTTP layer. Implementations must return any HTTP status as a
    /// response (including 3xx — redirects are not followed) and throw
    /// <see cref="TransportException"/> for transport failures.
    /// </summary>
    public interface IHttpTransport
    {
        HttpResponseData Send(HttpRequestData request);
    }

    /// <summary>Default transport on top of <see cref="HttpClient"/>.</summary>
    public sealed class HttpClientTransport : IHttpTransport, IDisposable
    {
        private readonly object _lock = new object();
        private HttpClient _verifying;
        private HttpClient _nonVerifying;

        public HttpResponseData Send(HttpRequestData request)
        {
            var client = GetClient(request.VerifySsl, request.ConnectTimeout);
            using (var message = new HttpRequestMessage(new HttpMethod(request.Method), request.Url))
            {
                string contentType = null;
                foreach (var h in request.Headers)
                {
                    if (string.Equals(h.Key, "Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        contentType = h.Value;
                        continue;
                    }
                    message.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }
                if (request.Body != null)
                {
                    message.Content = new StringContent(request.Body, Encoding.UTF8);
                    message.Content.Headers.Remove("Content-Type");
                    message.Content.Headers.TryAddWithoutValidation("Content-Type", contentType ?? "application/json");
                }

                var started = DateTime.UtcNow;
                using (var cts = new CancellationTokenSource(request.Timeout))
                {
                    try
                    {
#if NET5_0_OR_GREATER
                        using (var response = client.Send(message, HttpCompletionOption.ResponseContentRead, cts.Token))
                        {
                            var body = response.Content.ReadAsStringAsync(cts.Token).GetAwaiter().GetResult();
                            return Build(response, body, DateTime.UtcNow - started);
                        }
#else
                        using (var response = client.SendAsync(message, HttpCompletionOption.ResponseContentRead, cts.Token).GetAwaiter().GetResult())
                        {
                            var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            return Build(response, body, DateTime.UtcNow - started);
                        }
#endif
                    }
                    catch (OperationCanceledException ex)
                    {
                        var ms = (long)request.Timeout.TotalMilliseconds;
                        throw new TransportException(28, "Operation timed out after " + ms + " milliseconds", ex);
                    }
                    catch (HttpRequestException ex)
                    {
                        throw Map(ex);
                    }
                    catch (IOException ex)
                    {
                        throw new TransportException(56, "Recv failure: " + ex.Message, ex);
                    }
                }
            }
        }

        private static HttpResponseData Build(HttpResponseMessage response, string body, TimeSpan elapsed)
        {
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var h in response.Headers) headers[h.Key] = string.Join(", ", h.Value);
            foreach (var h in response.Content.Headers) headers[h.Key] = string.Join(", ", h.Value);
            return new HttpResponseData
            {
                StatusCode = (int)response.StatusCode,
                Body = body ?? "",
                Headers = headers,
                ContentType = response.Content.Headers.ContentType?.ToString() ?? "",
                Elapsed = elapsed,
            };
        }

        private static TransportException Map(HttpRequestException ex)
        {
            for (Exception e = ex; e != null; e = e.InnerException)
            {
                if (e is SocketException se)
                {
                    switch (se.SocketErrorCode)
                    {
                        case SocketError.HostNotFound:
                        case SocketError.NoData:
                        case SocketError.TryAgain:
                            return new TransportException(6, "Could not resolve host: " + se.Message, ex);
                        case SocketError.TimedOut:
                            return new TransportException(28, "Connection timed out: " + se.Message, ex);
                        case SocketError.ConnectionReset:
                            return new TransportException(56, "Connection reset by peer", ex);
                        default:
                            return new TransportException(7, "Failed to connect: " + se.Message, ex);
                    }
                }
                if (e is AuthenticationException)
                {
                    return new TransportException(60, "SSL certificate problem: " + e.Message, ex);
                }
                if (e is IOException)
                {
                    return new TransportException(56, "Recv failure: " + e.Message, ex);
                }
            }
            return new TransportException(7, ex.Message, ex);
        }

        private HttpClient GetClient(bool verify, TimeSpan connectTimeout)
        {
            lock (_lock)
            {
                if (verify)
                {
                    return _verifying ?? (_verifying = Create(true, connectTimeout));
                }
                return _nonVerifying ?? (_nonVerifying = Create(false, connectTimeout));
            }
        }

        private static HttpClient Create(bool verify, TimeSpan connectTimeout)
        {
#if NET5_0_OR_GREATER
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false,
                ConnectTimeout = connectTimeout,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            };
            if (!verify)
            {
                handler.SslOptions.RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true;
            }
#else
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
            if (!verify)
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }
#endif
            // Per-request timeouts are enforced with a CancellationTokenSource.
            return new HttpClient(handler) { Timeout = System.Threading.Timeout.InfiniteTimeSpan };
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _verifying?.Dispose();
                _nonVerifying?.Dispose();
                _verifying = null;
                _nonVerifying = null;
            }
        }
    }
}
