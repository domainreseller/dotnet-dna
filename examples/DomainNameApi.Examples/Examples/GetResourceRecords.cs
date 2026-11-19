using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>All DNS zone records, one row per value.</summary>
    internal static class GetResourceRecords
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.GetResourceRecords("example.com");

            /*
             * {
             *   "result": "OK",
             *   "data": { "records": [
             *     { "Name": "example.com.", "Type": "A", "TTL": 3600, "Content": "192.0.2.1", "Disabled": false },
             *     { "Name": "www.example.com.", "Type": "CNAME", "TTL": 300, "Content": "example.com.", "Disabled": false },
             *     { "Name": "example.com.", "Type": "MX", "TTL": 3600, "Content": "10 mx1.example.com.", "Disabled": false } ] }
             * }
             */
            return result;
        }
    }
}
