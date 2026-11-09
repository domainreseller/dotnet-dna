using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Add one value to a set, keeping the others.</summary>
    internal static class AddResourceRecordValue
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.AddResourceRecordValue("example.com", "@", "MX", "30 mx3.example.com");

            /*
             * { "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com.", "20 mx2.example.com.", "30 mx3.example.com" ] } }
             */
            return result;
        }
    }
}
