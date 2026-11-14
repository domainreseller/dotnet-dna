using System;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Xunit;

namespace DomainNameApi.Tests
{
    public class HttpLayerTests
    {
        private static (DomainNameApiClient Client, FakeTransport Transport) Make(bool testMode = false)
        {
            var t = new FakeTransport();
            var c = new DomainNameApiClient(" reseller-id \n", "\tkey ", testMode, t) { ZoneMinInterval = TimeSpan.Zero };
            c.SetErrorReportingEnabled(false);
            return (c, t);
        }

        [Fact]
        public void SendsAuthHeadersUserAgentAndTrimmedCredentials()
        {
            var (c, t) = Make();
            t.Enqueue(200, "{\"resellerId\":\"r\"}");
            c.GetResellerDetails();

            var req = t.Requests[0];
            Assert.Equal("GET", req.Method);
            Assert.Equal("https://api.domainresellerapi.com/api/v1/deposit/accounts/me", req.Url);
            Assert.Equal("key", req.Headers["X-API-KEY"]);
            Assert.Equal("reseller-id", req.Headers["__reseller"]);
            Assert.Equal("DomainNameAPI-DotNet/1.0.1", req.Headers["User-Agent"]);
            Assert.Equal("application/json", req.Headers["Accept"]);
            Assert.Equal("application/json", req.Headers["Content-Type"]);
            Assert.Null(req.Body);
            Assert.Equal(TimeSpan.FromSeconds(30), req.Timeout);
            Assert.Equal(TimeSpan.FromSeconds(10), req.ConnectTimeout);
            Assert.True(req.VerifySsl);
        }

        [Fact]
        public void TestModeUsesOte()
        {
            Assert.Equal(DomainNameApiClient.UrlOte, Make(true).Client.GetServiceUrl());
            Assert.Equal(DomainNameApiClient.UrlProd, Make(false).Client.GetServiceUrl());
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("1", true)]
        [InlineData(" ON ", true)]
        [InlineData("Yes", true)]
        [InlineData("false", false)]
        [InlineData("off", false)]
        [InlineData("0", false)]
        [InlineData("", false)]
        [InlineData("anything", false)]
        public void StringTestModeFollowsFilterValidateBoolean(string flag, bool ote)
        {
            var c = new DomainNameApiClient("r", "k", flag);
            Assert.Equal(ote ? DomainNameApiClient.UrlOte : DomainNameApiClient.UrlProd, c.GetServiceUrl());
        }

        [Fact]
        public void SetServiceUrlAndSslVerification()
        {
            var (c, t) = Make();
            c.SetServiceUrl("http://127.0.0.1:9999/api/v1");
            c.SetSslVerification(false);
            t.Enqueue(200, "");
            c.EnableTheftProtectionLock("example.com");
            Assert.Equal("http://127.0.0.1:9999/api/v1/domains/lock", t.Requests[0].Url);
            Assert.False(t.Requests[0].VerifySsl);
            Assert.Equal("{\"domainName\":\"example.com\"}", t.Requests[0].Body);
        }

        [Fact]
        public void GetQueryUsesHttpBuildQuerySemantics()
        {
            var (c, t) = Make();
            t.Enqueue(200, "{\"items\":[]}");
            c.GetList(new System.Collections.Generic.Dictionary<string, object>
            {
                ["Flag"] = true,
                ["Off"] = false,
                ["Skip"] = null,
                ["Name"] = "a b~*",
                ["Ids"] = new[] { 1, 2 },
            });
            Assert.Equal(
                "https://api.domainresellerapi.com/api/v1/domains?MaxResultCount=200&SkipCount=0&Flag=1&Off=0&Name=a+b%7E%2A&Ids%5B0%5D=1&Ids%5B1%5D=2",
                t.Requests[0].Url);
        }

        [Fact]
        public void TimeoutBecomesErrorEnvelopeNotEmptySuccess()
        {
            var (c, t) = Make();
            t.EnqueueFailure(new TransportException(28, "Operation timed out after 30000 milliseconds"));
            var r = c.ModifyNameServer("example.com", new[] { "ns1.example.com" });

            Assert.Equal("ERROR", (string)r["result"]);
            Assert.Equal("CURL_28", (string)r["error"]["Code"]);
            Assert.Null(r["error"]["GatewayCode"]);
            var msg = "Could not reach the DomainNameAPI service: the connection timed out. (Operation timed out after 30000 milliseconds)";
            Assert.Equal(msg, (string)r["error"]["Message"]);
            Assert.Equal(msg, (string)r["error"]["Details"]);
            Assert.Equal("CURL_28", (string)c.GetParsedResponseData()["Code"]);
        }

        [Fact]
        public void OtherTransportFailuresUseCurlErrorMessage()
        {
            var (c, t) = Make();
            t.EnqueueFailure(new TransportException(6, "Could not resolve host: api.domainresellerapi.com"));
            var r = c.GetDetails("example.com");
            Assert.Equal("CURL_6", (string)r["error"]["Code"]);
            Assert.Equal("Curl error during request: Could not resolve host: api.domainresellerapi.com", (string)r["error"]["Message"]);
        }

        [Fact]
        public void RecordsDiagnostics()
        {
            var (c, t) = Make();
            t.Enqueue(404, "{\"error\":{\"code\":\"X\",\"message\":\"Domain not found\"}}");
            c.GetDetails("missing.com");

            Assert.Equal("request:GET domains/info", c.GetLastFunction());
            var req = c.GetRequestData();
            Assert.Equal("GET", (string)req["method"]);
            Assert.Equal("missing.com", (string)req["payload"]["DomainName"]);
            Assert.Equal(404, (int)c.GetResponseHeaders()["http_code"]);
            Assert.Equal("Domain not found", (string)c.GetResponseData()["error"]["message"]);
        }

        [Fact]
        public void LastFunctionDropsTheQueryString()
        {
            var (c, t) = Make();
            t.Enqueue(200, "");
            c.ApplyResourceRecords("example.com");
            Assert.Equal("request:POST domains/zones/apply", c.GetLastFunction());
            Assert.Equal("[]", t.Requests[0].Body);
        }

        [Fact]
        public void ZoneCallsArePaced()
        {
            var t = new FakeTransport();
            var c = new DomainNameApiClient("r", "k", false, t) { ZoneMinInterval = TimeSpan.FromMilliseconds(300) };
            c.SetErrorReportingEnabled(false);
            t.Enqueue(200, "[]").Enqueue(200, "[]").Enqueue(200, "{}");

            var sw = Stopwatch.StartNew();
            c.GetResourceRecords("example.com");
            c.GetResourceRecords("example.com");
            Assert.True(sw.ElapsedMilliseconds >= 250, "second zone call should wait");

            sw.Restart();
            c.GetDetails("example.com"); // not a zone call
            Assert.True(sw.ElapsedMilliseconds < 250, "non-zone calls are not paced");
        }

        [Fact]
        public void ErrorsAreReturnedNeverThrown()
        {
            var t = new FakeTransport(); // empty queue: the fake throws InvalidOperationException
            var c = new DomainNameApiClient("r", "k", false, t);
            c.SetErrorReportingEnabled(false);
            var r = c.GetDetails("example.com");
            Assert.Equal("ERROR", (string)r["result"]);
            Assert.Equal("DOMAIN_DETAILS", (string)r["error"]["Code"]);
        }
    }
}
