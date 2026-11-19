using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Complete and normalize a contact the way registration does (no network).</summary>
    internal static class ValidateContact
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.ValidateContact(new Dictionary<string, object>
            {
                ["FirstName"] = " Jo ",
                ["Phone"] = "+1 (202) 555-0100",
                ["Country"] = "Turkey",
                ["Fax"] = "0090 212 123 45 67",
            });

            /*
             * { "FirstName": "Jo", "Phone": "12025550100", "Country": "TR", "Fax": "2121234567", "LastName": "Isimyok",
             *   "AddressLine1": "Addres yok", "City": "ISTANBUL", "State": "ISTANBUL", "ZipCode": "34000",
             *   "PhoneCountryCode": "90", "FaxCountryCode": "90" }
             */
            return result;
        }
    }
}
