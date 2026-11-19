using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Make a name+type set hold exactly these values.</summary>
    internal static class SetResourceRecordSet
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.SetResourceRecordSet("example.com", "@", "MX", new[] { "10 mx1.example.com", "20 mx2.example.com" }, 3600);

            /*
             * { "result": "OK", "data": { "Name": "", "Type": "MX", "TTL": 3600, "Contents": [ "10 mx1.example.com", "20 mx2.example.com" ] } }
             */
            return result;
        }
    }
}
