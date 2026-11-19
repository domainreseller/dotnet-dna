using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Remove one value from a set, keeping the others.</summary>
    internal static class RemoveResourceRecordValue
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.RemoveResourceRecordValue("example.com", "@", "MX", "20 mx2.example.com.");

            /*
             * { "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com." ] } }
             */
            return result;
        }
    }
}
