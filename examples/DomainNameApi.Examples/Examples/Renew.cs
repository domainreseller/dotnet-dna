using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Renew a domain.</summary>
    internal static class Renew
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.Renew("example.com", 1);

            /*
             * { "result": "OK", "data": { "ExpirationDate": "2027-11-09T12:00:00" } }
             */
            return result;
        }
    }
}
