using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Read URL forwarding.</summary>
    internal static class GetForwarding
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.GetForwarding("example.com");

            /*
             * { "result": "OK", "data": { "Enabled": true, "RedirectAddress": "https://www.example.org/landing", "ForwardType": "Frame" } }
             */
            return result;
        }
    }
}
