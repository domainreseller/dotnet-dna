using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Refresh the domain from the registry (same envelope as GetDetails).</summary>
    internal static class SyncFromRegistry
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.SyncFromRegistry("example.com");

            /*
             * { "data": { "ID": 123456, "Status": "Active", "DomainName": "example.com", ... }, "result": "OK" }
             */
            return result;
        }
    }
}
