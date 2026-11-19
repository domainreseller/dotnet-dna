using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Change the IP address of a child nameserver.</summary>
    internal static class ModifyChildNameServer
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.ModifyChildNameServer("example.com", "ns1.example.com", "192.0.2.20");

            /*
             * { "result": "OK", "data": { "NameServer": "ns1.example.com", "IPAdresses": [ "192.0.2.20" ] } }
             */
            return result;
        }
    }
}
