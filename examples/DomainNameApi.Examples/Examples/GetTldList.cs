using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>TLD list with the pricing matrix.</summary>
    internal static class GetTldList
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.GetTldList(100);

            /*
             * {
             *   "data": [ { "id": 1, "status": "Active", "maxchar": 63, "maxperiod": 10, "minchar": 1, "minperiod": 1, "tld": "com",
             *     "pricing": { "registration": { "1": "10.8100" }, "renew": { "1": "11.0100" }, "transfer": { "1": "10.6100" } },
             *     "currencies": { "registration": "USD", "renew": "USD", "transfer": "USD" } } ],
             *   "result": "OK"
             * }
             */
            return result;
        }
    }
}
