using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Turn WHOIS privacy on or off.</summary>
    internal static class ModifyPrivacyProtectionStatus
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.ModifyPrivacyProtectionStatus("example.com", true, "Owner request");

            /*
             * { "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
             */
            return result;
        }
    }
}
