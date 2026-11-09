using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Add a record set (replaces an existing name+type set) and apply.</summary>
    internal static class AddResourceRecord
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.AddResourceRecord("example.com", "www", "A", "192.0.2.5", 300);

            /*
             * { "result": "OK", "data": { "Name": "www", "Type": "A", "TTL": 300, "Content": "192.0.2.5" } }
             */
            return result;
        }
    }
}
