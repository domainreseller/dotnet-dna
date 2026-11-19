using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Remove URL forwarding.</summary>
    internal static class DeleteForwarding
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.DeleteForwarding("example.com");

            /*
             * { "result": "OK", "data": { "DomainName": "example.com", "Removed": true } }
             */
            return result;
        }
    }
}
