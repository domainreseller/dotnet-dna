using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Reseller account details with USD and TL balances.</summary>
    internal static class GetResellerDetails
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.GetResellerDetails();

            /*
             * {
             *   "result": "OK", "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "active": true, "name": "example-reseller",
             *   "balance": "601.64", "currency": "USD", "symbol": "$",
             *   "balances": [ { "balance": "601.64", "currency": "USD", "symbol": "$" }, { "balance": "1250.5", "currency": "TL", "symbol": "TL" } ]
             * }
             */
            return result;
        }
    }
}
