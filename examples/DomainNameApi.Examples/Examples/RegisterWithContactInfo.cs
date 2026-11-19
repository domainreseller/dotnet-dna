using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Register a domain with full contact information.</summary>
    internal static class RegisterWithContactInfo
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

            var result = dna.RegisterWithContactInfo(
                "example.com",
                1,
                new Dictionary<string, object>
                {
                    ["Administrative"] = contact,
                    ["Billing"] = contact,
                    ["Technical"] = contact,
                    ["Registrant"] = contact,
                },
                new[] { "ns1.example.com", "ns2.example.com" },
                eppLock: true,
                privacyLock: false);

            /*
             * {
             *   "data": { "ID": 0, "Status": "Active", "DomainName": "", "AuthCode": "", "LockStatus": "false",
             *     "PrivacyProtectionStatus": "false", "IsChildNameServer": "false",
             *     "Contacts": { "Billing": { "ID": 0 }, "Technical": { "ID": 0 }, "Administrative": { "ID": 0 }, "Registrant": { "ID": 0 } },
             *     "Dates": { "Start": "", "Expiration": "2027-11-09T12:00:00", "RemainingDays": 365 },
             *     "NameServers": [], "Additional": [], "ChildNameServers": [] },
             *   "result": "OK"
             * }
             */
            return result;
        }
    }
}
