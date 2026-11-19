using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Turn WHOIS privacy on or off.</summary>
    internal static class ModifyPrivacyProtectionStatus
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            // The flag may also be text ("1", "true", "on", "yes"); the reason argument is not sent.
            var result = dna.ModifyPrivacyProtectionStatus("example.com", true);

            /*
             * { "data": { "PrivacyProtectionStatus": true }, "result": "OK" }
             */
            return result;
        }
    }
}
