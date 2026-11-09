using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>Check whether a domain can be transferred with an auth code.</summary>
    internal static class CheckTransfer
    {
        public static JsonNode Run(DomainNameApiClient dna)
        {
            var result = dna.CheckTransfer("example.com", "Ab1!xyZ9");

            /*
             * {
             *   "result": "OK",
             *   "data": { "TransferAvailabilityStatus": true, "AuthCodeIsRequired": true, "AuthCodeIsValid": true,
             *     "UserTransferRequired": false, "TransferLock": false, "Message": "Domain can be transferred",
             *     "MessageKey": "TransferAvailable", "Additional": [] }
             * }
             */
            return result;
        }
    }
}
