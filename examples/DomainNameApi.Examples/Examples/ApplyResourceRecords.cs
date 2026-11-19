using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Commit staged zone changes (after calls with apply: false).</summary>
    internal static class ApplyResourceRecords
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            dna.AddResourceRecord("example.com", "api", "A", "192.0.2.7", 300, apply: false);
            dna.AddResourceRecord("example.com", "cdn", "CNAME", "cdn.example.net.", 300, apply: false);
            var result = dna.ApplyResourceRecords("example.com");

            /*
             * { "result": "OK", "data": { "DomainName": "example.com" } }
             */
            return result;
        }
    }
}
