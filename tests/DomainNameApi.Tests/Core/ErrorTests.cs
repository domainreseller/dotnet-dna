using System.Net.Sockets;
using System.Text;

namespace DomainNameApi.Tests.Core;

public class ErrorTests
{
    [Theory]
    [InlineData(302)]
    [InlineData(301)]
    public async Task RedirectMeansAnUnknownOrSuspendedReseller(int status)
    {
        using var t = new TestClient();
        t.Handler.Respond(status, "", ("Location", "/Error/Suspend"));

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Equal(ApiErrorKind.Authentication, e.Kind);
        Assert.Equal("INVALID_RESELLER", e.Code);
        Assert.Contains("Environment", e.Hint);
    }

    [Fact]
    public async Task AFollowedRedirectToTheErrorPageIsStillAnAuthenticationError()
    {
        using var t = new TestClient();
        t.Handler.Enqueue((request, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<html>Suspended</html>", Encoding.UTF8, "text/html"),
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://ote.domainresellerapi.com/Error/Suspend"),
        }));

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Equal("INVALID_RESELLER", e.Code);
    }

    [Theory]
    [InlineData("API key is missing", "MISSING_API_KEY")]
    [InlineData("ResellerId is missing", "MISSING_RESELLER_ID")]
    [InlineData("Invalid API key", "INVALID_API_KEY")]
    [InlineData("You do not have authorization", "SUBRESELLER_NOT_ALLOWED")]
    [InlineData("Nope", "UNAUTHORIZED")]
    public async Task ClassifiesUnauthorizedAnswers(string message, string code)
    {
        using var t = new TestClient();
        t.Handler.Respond(401, message);

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Equal(ApiErrorKind.Authentication, e.Kind);
        Assert.Equal(code, e.Code);
        Assert.Equal(401, e.StatusCode);
    }

    [Fact]
    public async Task ReadsTheGatewayErrorWithValidationErrors()
    {
        using var t = new TestClient();
        t.Handler.Respond(400, """
            {"error":{"code":"Dna.DomainService:Domain:1018","message":"Your request is not valid!","details":"The following errors were detected.",
             "validationErrors":[{"message":"The eMail field is not a valid e-mail address.","members":["contacts[0].eMail"]},"Period is out of range"]}}
            """, ("X-Correlation-Id", "corr-1"));

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Domains.RenewAsync("example.com"));

        Assert.Equal(ApiErrorKind.Validation, e.Kind);
        Assert.Equal("Dna.DomainService:Domain:1018", e.Code);
        Assert.Equal("Your request is not valid!", e.Message);
        Assert.Equal("The following errors were detected.", e.Details);
        Assert.Equal("corr-1", e.CorrelationId);
        Assert.Equal("POST domains/renew", e.Operation);
        Assert.Equal(2, e.ValidationErrors.Count);
        Assert.Equal(new[] { "contacts[0].eMail" }, e.ValidationErrors[0].Members);
        Assert.Equal("Period is out of range", e.ValidationErrors[1].Message);
        Assert.False(e.IsRetryable);
        Assert.Contains("contacts[0].eMail", e.ToString());
    }

    [Fact]
    public async Task ReadsAFlatErrorObject()
    {
        using var t = new TestClient();
        t.Handler.Respond(404, """{"code":"ERR_DOMAIN_NOT_FOUND","message":"Domain not found"}""");

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Domains.GetAsync("missing.com"));

        Assert.Equal(ApiErrorKind.NotFound, e.Kind);
        Assert.Equal("ERR_DOMAIN_NOT_FOUND", e.Code);
        Assert.Equal("Domain not found", e.Message);
        Assert.NotNull(e.Hint);
    }

    [Fact]
    public async Task ReplacesBoilerplateWithTheDetails()
    {
        using var t = new TestClient();
        t.Handler.Respond(400, """{"error":{"code":"Dna:1","message":"Exception of type 'Volo.Abp.BusinessException' was thrown.","details":"Dna Domain is locked"}}""");

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Domains.RenewAsync("example.com"));

        Assert.Equal("Domain is locked", e.Message);
    }

    [Fact]
    public async Task ReplacesAnEchoedCodeWithAGenericMessage()
    {
        using var t = new TestClient();
        t.Handler.Respond(400, """{"error":{"code":"Dna:7","message":"Dna:7"}}""");

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Domains.RenewAsync("example.com"));

        Assert.Equal("The operation could not be completed (error Dna:7).", e.Message);
    }

    [Theory]
    [InlineData(500, ApiErrorKind.Server, true)]
    [InlineData(501, ApiErrorKind.Server, false)]
    [InlineData(403, ApiErrorKind.Forbidden, false)]
    [InlineData(409, ApiErrorKind.Conflict, false)]
    [InlineData(422, ApiErrorKind.Validation, false)]
    [InlineData(418, ApiErrorKind.Unknown, false)]
    public async Task ClassifiesStatusCodes(int status, ApiErrorKind kind, bool retryable)
    {
        using var t = new TestClient(o => o.MaxRetries = 0);
        t.Handler.Respond(status, "");

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Equal(kind, e.Kind);
        Assert.Equal("HTTP_" + status, e.Code);
        Assert.Equal(retryable, e.IsRetryable);
        Assert.StartsWith("HTTP " + status, e.Message);
        Assert.Null(e.ResponseBody);
    }

    [Fact]
    public async Task KeepsATextBodyAsTheMessage()
    {
        using var t = new TestClient(o => o.MaxRetries = 0);
        t.Handler.Respond(500, "<html>Bad gateway</html>");

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Equal("<html>Bad gateway</html>", e.Message);
        Assert.Equal("<html>Bad gateway</html>", e.ResponseBody);
    }

    [Fact]
    public async Task AnEmptySuccessfulAnswerIsAnError()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, "");

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Equal("EMPTY_RESPONSE", e.Code);
    }

    [Fact]
    public async Task AnUnreadableSuccessfulAnswerIsAnError()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, "<html>maintenance</html>");

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Equal("INVALID_RESPONSE", e.Code);
        Assert.IsAssignableFrom<JsonException>(e.InnerException);
    }

    [Fact]
    public async Task ANoContentRequestIgnoresTheBody()
    {
        using var t = new TestClient();
        t.Handler.Respond(200, "not json");

        await t.Client.Domains.LockAsync("example.com");
    }

    [Fact]
    public async Task NetworkFailureOfABillableRequestHasAnUnknownOutcome()
    {
        using var t = new TestClient();
        t.Handler.Fail(new HttpRequestException("Connection reset", new SocketException((int)SocketError.ConnectionReset)));

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Domains.RenewAsync("example.com"));

        Assert.Equal(ApiErrorKind.Network, e.Kind);
        Assert.Equal("NETWORK", e.Code);
        Assert.True(e.OutcomeUnknown);
        Assert.True(e.IsRetryable);
        Assert.Contains("may have reached the API", e.Hint);
        Assert.IsType<HttpRequestException>(e.InnerException);
        Assert.Single(t.Handler.Requests); // never sent twice
    }

    [Fact]
    public async Task SlowAnswersTimeOut()
    {
        using var t = new TestClient(o =>
        {
            o.Timeout = TimeSpan.FromMilliseconds(50);
            o.MaxRetries = 0;
        });
        t.Handler.Hang();

        var e = await Assert.ThrowsAsync<DomainNameApiException>(() => t.Client.Account.GetAsync());

        Assert.Equal(ApiErrorKind.Timeout, e.Kind);
        Assert.Equal("TIMEOUT", e.Code);
        Assert.False(e.OutcomeUnknown); // a read changes nothing
    }
}
