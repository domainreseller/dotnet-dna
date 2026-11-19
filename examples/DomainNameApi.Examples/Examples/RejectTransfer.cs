using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Reject a pending outgoing transfer.</summary>
    internal static class RejectTransfer
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.RejectTransfer("example.com");

            /*
             * { "result": "OK", "data": { "DomainName": "example.com", "Status": "Rejected" } }
             */
            return result;
        }
    }
}
