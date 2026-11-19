using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Remove every value under a name+type.</summary>
    internal static class DeleteResourceRecordSet
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.DeleteResourceRecordSet("example.com", "@", "MX");

            /*
             * { "result": "OK", "data": { "Name": "example.com.", "Type": "MX", "Content": "10 mx1.example.com." } }
             */
            return result;
        }
    }
}
