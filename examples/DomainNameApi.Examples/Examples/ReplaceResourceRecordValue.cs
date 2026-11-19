using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Swap one value of a set for another.</summary>
    internal static class ReplaceResourceRecordValue
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.ReplaceResourceRecordValue("example.com", "@", "A", "192.0.2.1", "192.0.2.2");

            /*
             * { "result": "OK", "data": { "Name": "", "Type": "A", "TTL": 3600, "Contents": [ "192.0.2.2" ] } }
             */
            return result;
        }
    }
}
