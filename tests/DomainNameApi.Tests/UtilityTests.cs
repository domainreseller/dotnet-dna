using System.Collections.Generic;
using System.Text.Json.Nodes;
using DomainNameApi.Internal;
using Xunit;

namespace DomainNameApi.Tests
{
    public class UtilityTests
    {
        private static DomainNameApiClient Client() => new DomainNameApiClient("r", "k", false, new FakeTransport());

        [Theory]
        [InlineData("example.com.tr", true)]
        [InlineData("EXAMPLE.TR", true)]
        [InlineData("example.com", false)]
        [InlineData("example.trade", false)]
        [InlineData("tr", false)]
        [InlineData("", false)]
        public void IsTrTLD(string domain, bool expected) => Assert.Equal(expected, Client().IsTrTLD(domain));

        [Fact]
        public void ValidateContactFillsDefaultsAndNormalizes()
        {
            var r = Client().ValidateContact(new Dictionary<string, object>
            {
                ["FirstName"] = " Jo 2nd ",
                ["Phone"] = "+90 (532) 374 33 88",
                ["PhoneCountryCode"] = 90,
                ["Country"] = "germany",
                ["City"] = "Berlin",
            });
            Assert.Equal("Jo nd", (string)r["FirstName"]);
            Assert.Equal("Isimyok", (string)r["LastName"]);
            Assert.Equal("5323743388", (string)r["Phone"]);
            Assert.Equal("90", (string)r["PhoneCountryCode"]);
            Assert.Equal("DE", (string)r["Country"]);
            // A missing State gets the default; only a present-but-blank State falls back to the city.
            Assert.Equal("ISTANBUL", (string)r["State"]);
            Assert.Equal("", (string)r["Fax"]);
            Assert.Equal("", (string)r["FaxCountryCode"]);
        }

        [Theory]
        [InlineData("0532 374 33 88", "", "5323743388", "90")]
        [InlineData("905323743388", "", "5323743388", "90")]
        [InlineData("905323743388", "90", "5323743388", "90")]
        [InlineData("+1 202 555 0100", "1", "2025550100", "1")]
        [InlineData("0044 20 7123 4567", "", "442071234567", "90")]
        [InlineData("", "44", "", "44")]
        public void NormalizePhone(string number, string cc, string expectedNumber, string expectedCc)
        {
            var (n, c) = DomainNameApiClient.NormalizePhone(number, cc);
            Assert.Equal(expectedNumber, n);
            Assert.Equal(expectedCc, c);
        }

        [Theory]
        [InlineData("Şükrü Öztürk", "Şükrü Öztürk")]
        [InlineData("O'Neil-Smith 3rd", "O'Neil-Smith rd")]
        [InlineData("1234", "")]
        public void SanitizePersonName(string input, string expected) => Assert.Equal(expected, DomainNameApiClient.SanitizePersonName(input));

        [Theory]
        [InlineData("tr", "TR")]
        [InlineData(" Türkiye ", "TR")]
        [InlineData("United States", "US")]
        [InlineData("Narnia", "Narnia")]
        [InlineData("", "")]
        public void NormalizeCountryCode(string input, string expected) => Assert.Equal(expected, DomainNameApiClient.NormalizeCountryCode(input));

        [Theory]
        [InlineData("192.0.2.1", true)]
        [InlineData("255.255.255.255", true)]
        [InlineData("256.1.1.1", false)]
        [InlineData("01.2.3.4", false)]
        [InlineData("2001:db8::1", false)]
        [InlineData("", false)]
        public void IsIpv4(string ip, bool expected) => Assert.Equal(expected, DomainNameApiClient.IsIpv4(ip));

        [Theory]
        [InlineData(601.64, "601.64")]
        [InlineData(0.1 + 0.2, "0.3")]
        [InlineData(1e15, "1.0E+15")]
        [InlineData(1e-5, "1.0E-5")]
        [InlineData(100.0, "100")]
        [InlineData(1.2345678901234567e17, "1.2345678901235E+17")]
        public void FloatToStringMatchesPhp(double value, string expected) => Assert.Equal(expected, Php.FloatToString(value));

        [Theory]
        [InlineData(1.005, 2, "1.01")]
        [InlineData(11.99, 4, "11.9900")]
        [InlineData(-0.001, 2, "0.00")]
        [InlineData(2500, 4, "2500.0000")]
        [InlineData(1250.5, 2, "1250.50")]
        public void NumberFormatMatchesPhp(double value, int decimals, string expected) => Assert.Equal(expected, Php.NumberFormat(value, decimals));

        [Theory]
        [InlineData(" 12abc", 12)]
        [InlineData("1e3", 1000)]
        [InlineData("1.9", 1)]
        [InlineData("abc", 0)]
        [InlineData("-7", -7)]
        public void StringToIntMatchesPhp(string s, long expected) => Assert.Equal(expected, Php.StringToInt(s));

        [Theory]
        [InlineData("12", true)]
        [InlineData(" 12 ", true)]
        [InlineData("1.", true)]
        [InlineData(".5", true)]
        [InlineData("1e5", true)]
        [InlineData(".", false)]
        [InlineData("", false)]
        [InlineData("0x1A", false)]
        [InlineData("ok", false)]
        public void IsNumericMatchesPhp(string s, bool expected) => Assert.Equal(expected, Php.IsNumericString(s));

        [Fact]
        public void EmptyMatchesPhp()
        {
            Assert.True(Php.Empty(null));
            Assert.True(Php.Empty(JsonValue.Create("0")));
            Assert.True(Php.Empty(JsonValue.Create("")));
            Assert.True(Php.Empty(JsonValue.Create(0L)));
            Assert.True(Php.Empty(JsonValue.Create(0.0)));
            Assert.True(Php.Empty(JsonValue.Create(false)));
            Assert.True(Php.Empty(new JsonArray()));
            Assert.True(Php.Empty(new JsonObject()));
            Assert.False(Php.Empty(JsonValue.Create("0.0")));
            Assert.False(Php.Empty(JsonValue.Create(1)));
        }

        [Fact]
        public void JsonEncodeMatchesPhpDefaults()
        {
            var n = JsonNode.Parse("{\"a/b\":\"ü/\",\"f\":1.0,\"e\":{}}");
            Assert.Equal("{\"a\\/b\":\"\\u00fc\\/\",\"f\":1.0,\"e\":[]}", Php.JsonEncode(Php.FromObject(n)));
        }

        [Theory]
        [InlineData("2027-03-15T10:20:30", true, "2027-03-15T10:20:30")]
        [InlineData("2027-03-15T10:20:30.1234567Z", true, "2027-03-15T10:20:30")]
        [InlineData("2027-03-15T13:20:30+03:00", true, "2027-03-15T10:20:30")]
        [InlineData("2027-03-15", true, "2027-03-15T00:00:00")]
        [InlineData("0001-01-01T00:00:00", true, "0001-01-01T00:00:00")]
        [InlineData("not-a-date", false, "")]
        public void ApiDates(string text, bool ok, string expected)
        {
            Assert.Equal(ok, DomainNameApiClient.TryParseApiDate(JsonValue.Create(text), out var utc));
            if (ok) Assert.Equal(expected, utc.ToString("yyyy-MM-dd'T'HH:mm:ss"));
        }

        [Fact]
        public void ComparatorTreatsEmptyListAndObjectAsEqualAndNumbersByValue()
        {
            Assert.True(Json.Equal(JsonNode.Parse("{\"a\":[],\"b\":1}"), JsonNode.Parse("{\"b\":1.0,\"a\":{}}"), out _));
            Assert.False(Json.Equal(JsonNode.Parse("{\"a\":\"1\"}"), JsonNode.Parse("{\"a\":1}"), out _));
            Assert.False(Json.Equal(JsonNode.Parse("[1,2]"), JsonNode.Parse("[2,1]"), out _));
            Assert.False(Json.Equal(JsonNode.Parse("{\"a\":1}"), JsonNode.Parse("{\"a\":1,\"b\":null}"), out _));
        }
    }
}
