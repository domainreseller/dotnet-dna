using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Register a .tr domain with TRABIS attributes (individual or commercial owner).</summary>
    internal static class RegisterWithContactInfoTR
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

            // Individual owner. Legacy keys (TRABISCITIZIENID, numeric category,
            // TRABISCOUNTRY*/CITY*/NAMESURNAME) are normalized for the gateway.
            var individual = new Dictionary<string, object>
            {
                ["TRABISDOMAINCATEGORY"] = 1,
                ["TRABISCITIZIENID"] = "11111111111",
                ["TRABISNAMESURNAME"] = "Ahmet Yilmaz",
                ["TRABISCOUNTRYID"] = "215",
                ["TRABISCITYID"] = "34",
            };

            // Company owner.
            var commercial = new Dictionary<string, object>
            {
                ["TRABISDOMAINCATEGORY"] = 0,
                ["TRABISORGANIZATION"] = "Example Ltd. Sti.",
                ["TRABISTAXOFFICE"] = "Istanbul VD",
                ["TRABISTAXNUMBER"] = "1234567890",
                ["TRABISCOUNTRYID"] = "215",
                ["TRABISCITYID"] = "34",
            };

            var result = dna.RegisterWithContactInfo(
                "example.com.tr",
                1,
                new Dictionary<string, object>
                {
                    ["Administrative"] = contact,
                    ["Billing"] = contact,
                    ["Technical"] = contact,
                    ["Registrant"] = contact,
                },
                new[] { "ns1.example.com", "ns2.example.com" },
                true,
                false,
                individual); // or: commercial

            /*
             * { "data": { "ID": 0, "Status": "Active", ..., "Dates": { "Start": "", "Expiration": "2027-11-09T12:00:00", "RemainingDays": 365 }, ... }, "result": "OK" }
             */
            return result;
        }
    }
}
