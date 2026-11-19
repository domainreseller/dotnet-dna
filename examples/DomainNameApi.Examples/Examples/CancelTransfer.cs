using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Cancel a pending incoming transfer.</summary>
    internal static class CancelTransfer
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.CancelTransfer("example.com");

            /*
             * { "result": "OK", "data": { "DomainName": "example.com", "Status": "Cancelled" } }
             */
            return result;
        }
    }
}
