using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Delete a child nameserver.</summary>
    internal static class DeleteChildNameServer
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.DeleteChildNameServer("example.com", "ns1.example.com");

            /*
             * { "result": "OK", "data": { "NameServer": "ns1.example.com" } }
             */
            return result;
        }
    }
}
