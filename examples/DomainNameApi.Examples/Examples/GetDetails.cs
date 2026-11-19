using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Domain details.</summary>
    internal static class GetDetails
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.GetDetails("example.com");

            /*
             * {
             *   "data": { "ID": 123456, "Status": "Active", "DomainName": "example.com", "AuthCode": "ABC123XYZ",
             *     "LockStatus": "true", "PrivacyProtectionStatus": "false", "IsChildNameServer": "false",
             *     "Contacts": { "Billing": { "ID": "DNA-B-4" }, "Technical": { "ID": "DNA-T-3" }, "Administrative": { "ID": "DNA-A-2" }, "Registrant": { "ID": "DNA-R-1" } },
             *     "Dates": { "Start": "2022-09-09T00:00:00", "Expiration": "2026-09-08T00:00:00", "RemainingDays": 1284 },
             *     "NameServers": [ "ns1.example.net", "ns2.example.net" ], "Additional": [], "ChildNameServers": [] },
             *   "result": "OK"
             * }
             */
            return result;
        }
    }
}
