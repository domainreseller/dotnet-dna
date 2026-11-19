using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Point the domain at a URL ("Standard" redirect or "Frame" masked).</summary>
    internal static class SetForwarding
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.SetForwarding("example.com", "https://www.example.org", "Standard");

            /*
             * { "result": "OK", "data": { "RedirectAddress": "https://www.example.org", "ForwardType": "Standard" } }
             */
            return result;
        }
    }
}
