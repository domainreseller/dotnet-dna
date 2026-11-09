using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Start an incoming transfer with the EPP/auth code.</summary>
    internal static class Transfer
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.Transfer("example.com", "Xy9#mK2$", 1);

            /*
             * { "data": { "ID": 0, "Status": "Active", "DomainName": "", ..., "Dates": { "Start": "", "Expiration": "2027-11-09T12:00:00", "RemainingDays": 365 }, ... }, "result": "OK" }
             */
            return result;
        }
    }
}
