using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Availability of every domain x extension pair (returns a list).</summary>
    internal static class CheckAvailability
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.CheckAvailability(new[] { "hello", "world123x0" }, new[] { "com", "net" }, 1, "create");

            /*
             * [
             *   { "TLD": "com", "DomainName": "hello", "Status": "notavailable", "Command": "create", "Period": 1,
             *     "IsFee": false, "Price": "10.8100", "Currency": "USD", "Reason": "Domain exists" },
             *   { "TLD": "com", "DomainName": "world123x0", "Status": "available", "Command": "create", "Period": 1,
             *     "IsFee": false, "Price": "10.8100", "Currency": "USD", "Reason": null }
             * ]
             */
            return result;
        }
    }
}
