using System.Text.Json.Nodes;
using Xunit;

namespace DomainNameApi.Tests
{
    public class ErrorMappingTests
    {
        private static JsonNode Call(int status, string body)
        {
            var t = new FakeTransport().Enqueue(status, body);
            var c = new DomainNameApiClient("r", "k", false, t);
            c.SetErrorReportingEnabled(false);
            return c.GetDetails("example.com");
        }

        [Fact]
        public void TopLevelMessageWins()
        {
            var r = Call(400, "{\"message\":\"Top\",\"error\":{\"message\":\"Nested\"},\"details\":\"D\"}");
            Assert.Equal("API_400", (string)r["error"]["Code"]);
            Assert.Equal("Top", (string)r["error"]["Message"]);
            Assert.Equal("D", (string)r["error"]["Details"]);
        }

        [Fact]
        public void EmptyBodySynthesisesStatusMessage()
        {
            var r = Call(503, "");
            Assert.Equal("API_503", (string)r["error"]["Code"]);
            Assert.Equal("HTTP 503 - Service Unavailable", (string)r["error"]["Message"]);
            Assert.Equal("HTTP 503 - Service Unavailable", (string)r["error"]["Details"]);
        }

        [Theory]
        [InlineData(418, "HTTP 418 - Client Error")]
        [InlineData(599, "HTTP 599 - Server Error")]
        [InlineData(304, "HTTP 304 - Unexpected Response")]
        public void UnknownStatusesUseClassLabels(int status, string expected)
        {
            Assert.Equal(expected, (string)Call(status, "")["error"]["Message"]);
        }

        [Fact]
        public void RedirectMeansBadCredentials()
        {
            var r = Call(301, "<html>moved</html>");
            Assert.Equal("API_301", (string)r["error"]["Code"]);
            Assert.Equal("Invalid API credentials", (string)r["error"]["Message"]);
            Assert.Equal("Authentication failed. Check your API key and reseller ID.", (string)r["error"]["Details"]);
        }

        [Fact]
        public void BoilerplateUsesDetailsWithoutDnaPrefix()
        {
            var r = Call(400, "{\"error\":{\"code\":\"C\",\"message\":\"Exception of type 'Volo.Abp.BusinessException' was thrown.\",\"details\":\"Dna Real reason\"}}");
            Assert.Equal("Real reason", (string)r["error"]["Message"]);
            Assert.Equal("Dna Real reason", (string)r["error"]["Details"]);
        }

        [Fact]
        public void EchoedCodeWithEmptyDetailsGetsSynthesisedMessage()
        {
            var r = Call(400, "{\"code\":\"Dna.X:1\",\"message\":\"Dna.X:1\",\"details\":\"Dna.X:1\"}");
            Assert.Equal("The operation could not be completed by the registry (error: Dna.X:1). Please try again or contact support.", (string)r["error"]["Message"]);
        }

        [Fact]
        public void EchoedCodeWithNullDetailsFallsBackToRawBody()
        {
            // php-dna behaviour (see addChildNameServer.error fixture): a null `details`
            // falls back to the raw body, which then replaces the echoed code.
            const string body = "{\"error\":{\"code\":\"Dna.X:1\",\"message\":\"Dna.X:1\",\"details\":null}}";
            var r = Call(400, body);
            Assert.Equal(body, (string)r["error"]["Message"]);
            Assert.Equal(body, (string)r["error"]["Details"]);
        }

        [Fact]
        public void ValidationErrorsAreFlattenedIntoDetails()
        {
            var body = "{\"error\":{\"message\":\"Validation failed.\",\"validationErrors\":[" +
                       "{\"members\":[\"$.administrativeContact.postalCode\",\"$.technicalContact.postalCode\"],\"message\":\"'PostalCode' is required\"}," +
                       "{\"members\":[\"$.tldAttributes.TRABISCOUNTRYID\"]}," +
                       "\"plain text\"," +
                       "{\"message\":\"only message\"}," +
                       "{\"members\":[\"$.administrativeContact.postalCode\",\"$.technicalContact.postalCode\"],\"message\":\"'PostalCode' is required\"}]}}";
            var r = Call(400, body);
            Assert.Equal("Admin PostalCode, Tech PostalCode - 'PostalCode' is required | TldAttributes TRABISCOUNTRYID | plain text | only message",
                (string)r["error"]["Details"]);
        }

        [Theory]
        [InlineData("$.administrativeContact.postalCode", "Admin PostalCode")]
        [InlineData("$.contacts[2].eMail", "Contacts EMail")]
        [InlineData("$.registrant/phone", "Registrant Phone")]
        [InlineData("   ", "")]
        [InlineData("$.", "")]
        public void HumanizeValidationMember(string member, string expected)
        {
            Assert.Equal(expected, DomainNameApiClient.HumanizeValidationMember(member));
        }

        [Fact]
        public void HtmlBodyIsMessageAndDetails()
        {
            var r = Call(502, "<html>bad</html>");
            Assert.Equal("<html>bad</html>", (string)r["error"]["Message"]);
            Assert.Equal("<html>bad</html>", (string)r["error"]["Details"]);
        }

        [Fact]
        public void EmptySuccessBodyIsResponseError()
        {
            var r = Call(200, "");
            Assert.Equal("RESPONSE", (string)r["error"]["Code"]);
            Assert.Equal("Invalid response received from server! Response is empty.", (string)r["error"]["Message"]);
        }
    }
}
