using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Create a child nameserver (glue record).</summary>
    internal static class AddChildNameServer
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.AddChildNameServer("example.com", "ns1.example.com", "192.0.2.10");

            /*
             * { "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "192.0.2.10" ] } }
             */
            return result;
        }
    }
}
