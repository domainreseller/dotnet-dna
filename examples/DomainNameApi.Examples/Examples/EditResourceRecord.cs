using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Edit a record set identified by its current name.</summary>
    internal static class EditResourceRecord
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.EditResourceRecord("example.com", "www", "www", "A", "192.0.2.6", 600);

            /*
             * { "result": "OK", "data": { "Name": "www", "Type": "A", "TTL": 600, "Content": "192.0.2.6" } }
             */
            return result;
        }
    }
}
