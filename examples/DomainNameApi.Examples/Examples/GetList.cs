using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Domains in the account, with paging and sorting.</summary>
    internal static class GetList
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.GetList(new Dictionary<string, object>
            {
                ["OrderColumn"] = "Id",
                ["OrderDirection"] = "ASC",
                ["PageNumber"] = 0,
                ["PageSize"] = 1000,
            });

            /*
             * {
             *   "data": { "Domains": [ { "ID": 564346, "Status": "Active", "DomainName": "example.com", "AuthCode": "",
             *     "LockStatus": "false", "PrivacyProtectionStatus": "false", "IsChildNameServer": "false",
             *     "Contacts": { "Billing": { "ID": "" }, "Technical": { "ID": "" }, "Administrative": { "ID": "" }, "Registrant": { "ID": "" } },
             *     "Dates": { "Start": "2024-03-15T10:00:00", "Expiration": "2025-03-15T10:00:00", "RemainingDays": 365 },
             *     "NameServers": [ "ns1.example.com", "ns2.example.com" ], "Additional": [], "ChildNameServers": [] } ] },
             *   "result": "OK", "TotalCount": 1
             * }
             */
            return result;
        }
    }
}
