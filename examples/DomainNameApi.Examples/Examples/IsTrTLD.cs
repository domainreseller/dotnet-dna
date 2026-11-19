using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Is this a .tr domain? (no network)</summary>
    internal static class IsTrTLD
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = JsonValue.Create(dna.IsTrTLD("example.com.tr"));

            /*
             * true
             */
            return result;
        }
    }
}
