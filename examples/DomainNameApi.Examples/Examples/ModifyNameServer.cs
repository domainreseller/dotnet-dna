using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Replace the nameservers of a domain.</summary>
    internal static class ModifyNameServer
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.ModifyNameServer("example.com", new[] { "ns1.example.com", "ns2.example.com" });

            /*
             * { "result": "OK", "data": { "NameServers": [ "ns1.example.com", "ns2.example.com" ] } }
             */
            return result;
        }
    }
}
