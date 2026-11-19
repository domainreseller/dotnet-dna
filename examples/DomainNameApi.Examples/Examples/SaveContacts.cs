using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Save all four contacts of a domain.</summary>
    internal static class SaveContacts
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var contact = new Dictionary<string, object>
            {
                ["FirstName"] = "Ahmet",
                ["LastName"] = "Yilmaz",
                ["Company"] = "",
                ["EMail"] = "ahmet.yilmaz@example.com",
                ["AddressLine1"] = "Ornek Sokak No: 123",
                ["AddressLine2"] = "Daire 4",
                ["AddressLine3"] = "",
                ["City"] = "Istanbul",
                ["Country"] = "TR",
                ["Fax"] = "2121234567",
                ["FaxCountryCode"] = "90",
                ["Phone"] = "5551234567",
                ["PhoneCountryCode"] = 90,
                ["Type"] = "Contact",
                ["ZipCode"] = "34000",
                ["State"] = "Istanbul",
            };

            var result = dna.SaveContacts("example.com", new Dictionary<string, object>
            {
                ["Administrative"] = contact,
                ["Billing"] = contact,
                ["Technical"] = contact,
                ["Registrant"] = contact,
            });

            /*
             * { "result": "OK", "data": { "contacts": [] } }
             */
            return result;
        }
    }
}
