using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Turn the registrar lock off.</summary>
    internal static class DisableTheftProtectionLock
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.DisableTheftProtectionLock("example.com");

            /*
             * { "result": "OK", "data": { "LockStatus": false } }
             */
            return result;
        }
    }
}
