using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DomainNameApi.Tests
{
    /// <summary>The default HttpClient transport against a loopback listener (no external network).</summary>
    public class HttpClientTransportTests
    {
        private sealed class Loopback : IDisposable
        {
            private readonly HttpListener _listener = new HttpListener();
            private readonly Task _loop;

            public Loopback(Func<HttpListenerContext, Task> handler)
            {
                var port = FreePort();
                BaseUrl = "http://127.0.0.1:" + port + "/";
                _listener.Prefixes.Add(BaseUrl);
                _listener.Start();
                _loop = Task.Run(async () =>
                {
                    while (_listener.IsListening)
                    {
                        HttpListenerContext ctx;
                        try { ctx = await _listener.GetContextAsync(); }
                        catch { return; }
                        _ = Task.Run(async () =>
                        {
                            try { await handler(ctx); }
                            catch { /* client went away */ }
                        });
                    }
                });
            }

            public string BaseUrl { get; }

            private static int FreePort()
            {
                var l = new TcpListener(IPAddress.Loopback, 0);
                l.Start();
                var port = ((IPEndPoint)l.LocalEndpoint).Port;
                l.Stop();
                return port;
            }

            public void Dispose()
            {
                try { _listener.Stop(); _listener.Close(); } catch { }
            }
        }

        private static async Task Reply(HttpListenerContext ctx, int status, string body, string location = null)
        {
            ctx.Response.StatusCode = status;
            if (location != null) ctx.Response.RedirectLocation = location;
            var bytes = Encoding.UTF8.GetBytes(body);
            ctx.Response.ContentType = "application/json";
            ctx.Response.ContentLength64 = bytes.Length;
            await ctx.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            ctx.Response.Close();
        }

        [Fact]
        public void SendsHeadersAndBodyAndReadsResponse()
        {
            string seenReseller = null, seenKey = null, seenAgent = null, seenBody = null, seenMethod = null, seenType = null;
            using (var server = new Loopback(async ctx =>
            {
                seenReseller = ctx.Request.Headers["__reseller"];
                seenKey = ctx.Request.Headers["X-API-KEY"];
                seenAgent = ctx.Request.UserAgent;
                seenMethod = ctx.Request.HttpMethod;
                seenType = ctx.Request.ContentType;
                using (var reader = new StreamReader(ctx.Request.InputStream)) seenBody = await reader.ReadToEndAsync();
                await Reply(ctx, 200, "{\"status\":\"Approved\"}");
            }))
            using (var client = new DomainNameApiClient("reseller", "key"))
            {
                client.SetErrorReportingEnabled(false);
                client.SetServiceUrl(server.BaseUrl + "api/v1");
                var r = client.ApproveTransfer("example.com");

                Assert.Equal("OK", (string)r["result"]);
                Assert.Equal("Approved", (string)r["data"]["Status"]);
                Assert.Equal("POST", seenMethod);
                Assert.Equal("reseller", seenReseller);
                Assert.Equal("key", seenKey);
                Assert.Equal("DomainNameAPI-DotNet/1.0.1", seenAgent);
                Assert.Equal("application/json", seenType);
                Assert.Equal("{\"domainName\":\"example.com\"}", seenBody);
            }
        }

        [Fact]
        public void RedirectsAreNotFollowed()
        {
            using (var server = new Loopback(ctx => Reply(ctx, 302, "", "http://127.0.0.1:1/login")))
            using (var client = new DomainNameApiClient("reseller", "key"))
            {
                client.SetErrorReportingEnabled(false);
                client.SetServiceUrl(server.BaseUrl + "api/v1");
                var r = client.GetResellerDetails();
                Assert.Equal("API_302", (string)r["error"]["Code"]);
                Assert.Equal("Invalid API credentials", (string)r["error"]["Message"]);
            }
        }

        [Fact]
        public void TimeoutMapsToCurl28()
        {
            using (var server = new Loopback(async ctx =>
            {
                await Task.Delay(3000);
                await Reply(ctx, 200, "{}");
            }))
            using (var client = new DomainNameApiClient("reseller", "key") { Timeout = TimeSpan.FromMilliseconds(300) })
            {
                client.SetErrorReportingEnabled(false);
                client.SetServiceUrl(server.BaseUrl + "api/v1");
                var r = client.GetDetails("example.com");
                Assert.Equal("CURL_28", (string)r["error"]["Code"]);
                Assert.StartsWith("Could not reach the DomainNameAPI service: the connection timed out.", (string)r["error"]["Message"]);
            }
        }

        [Fact]
        public void ConnectionRefusedIsATransportError()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();

            using (var client = new DomainNameApiClient("reseller", "key"))
            {
                client.SetErrorReportingEnabled(false);
                client.SetServiceUrl("http://127.0.0.1:" + port + "/api/v1");
                var r = client.EnableTheftProtectionLock("example.com");
                Assert.Equal("ERROR", (string)r["result"]);
                Assert.Equal("CURL_7", (string)r["error"]["Code"]);
                Assert.StartsWith("Curl error during request: ", (string)r["error"]["Message"]);
            }
        }
    }
}
