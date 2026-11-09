using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>The four contacts of a domain.</summary>
    internal static class GetContacts
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.GetContacts("example.com");

            /*
             * {
             *   "data": { "contacts": {
             *     "Administrative": { "ID": "DNA-A-2", "Status": "Active", "AuthCode": "", "FirstName": "Grace", "LastName": "Hopper",
             *       "Company": "", "EMail": "grace@example.com", "Type": "Administrative",
             *       "Address": { "Line1": "Example Street 2", "Line2": "", "Line3": "", "State": "DC", "City": "Washington", "Country": "US", "ZipCode": "20001" },
             *       "Phone": { "Phone": { "Number": "2025550100", "CountryCode": "1" }, "Fax": { "Number": "2025550101", "CountryCode": "1" } },
             *       "Additional": [] },
             *     "Billing": { ... }, "Registrant": { ... }, "Technical": { ... } } },
             *   "result": "OK"
             * }
             */
            return result;
        }
    }
}
