using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Balance in one currency: USD, TRY, EUR or GBP.</summary>
    internal static class GetCurrentBalance
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.GetCurrentBalance("USD");

            /*
             * {
             *   "ErrorCode": 0, "OperationMessage": "Command completed succesfully.", "OperationResult": "SUCCESS",
             *   "Balance": "601.64", "CurrencyId": 2, "CurrencyInfo": null, "CurrencyName": "USD", "CurrencySymbol": "$"
             * }
             */
            return result;
        }
    }
}
