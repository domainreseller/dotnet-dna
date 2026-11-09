using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Approve a pending outgoing transfer.</summary>
    internal static class ApproveTransfer
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.ApproveTransfer("example.com");

            /*
             * { "result": "OK", "data": { "DomainName": "example.com", "Status": "Approved" } }
             */
            return result;
        }
    }
}
