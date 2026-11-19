using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Turn the registrar lock on.</summary>
    internal static class EnableTheftProtectionLock
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.EnableTheftProtectionLock("example.com");

            /*
             * { "result": "OK", "data": { "LockStatus": true } }
             */
            return result;
        }
    }
}
