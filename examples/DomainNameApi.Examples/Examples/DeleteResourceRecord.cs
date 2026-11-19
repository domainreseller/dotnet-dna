using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Delete a record set (the whole name+type set).</summary>
    internal static class DeleteResourceRecord
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.DeleteResourceRecord("example.com", "www", "192.0.2.5", "A");

            /*
             * { "result": "OK", "data": { "Name": "www", "Type": "A", "Content": "192.0.2.5" } }
             */
            return result;
        }
    }
}
