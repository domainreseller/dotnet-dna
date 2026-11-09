using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DomainNameApi.Examples
{
    /// <summary>
    /// Runs one example against the gateway:
    ///
    ///   dotnet run --project examples/DomainNameApi.Examples -- GetDetails
    ///
    /// Credentials come from DNA_RESELLER_ID / DNA_API_KEY (and DNA_TEST_MODE=true for
    /// the OTE test environment), otherwise the placeholders below are used.
    /// </summary>
    internal static class Program
    {
        // Credentials: your Reseller ID (UUID) + API Key from Panel → API Settings.
        // Panel login credentials will NOT work.
        private const string ResellerId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"; // Reseller ID (UUID)
        private const string ApiKey = "your-api-key";                          // API Key

        private static readonly SortedDictionary<string, Func<DomainNameApiClient, JsonNode>> Examples =
            new SortedDictionary<string, Func<DomainNameApiClient, JsonNode>>(StringComparer.OrdinalIgnoreCase)
        {
            ["AddChildNameServer"] = AddChildNameServer.Run,
            ["AddResourceRecord"] = AddResourceRecord.Run,
            ["AddResourceRecordValue"] = AddResourceRecordValue.Run,
            ["ApplyResourceRecords"] = ApplyResourceRecords.Run,
            ["ApproveTransfer"] = ApproveTransfer.Run,
            ["CancelTransfer"] = CancelTransfer.Run,
            ["CheckAvailability"] = CheckAvailability.Run,
            ["CheckTransfer"] = CheckTransfer.Run,
            ["DeleteChildNameServer"] = DeleteChildNameServer.Run,
            ["DeleteForwarding"] = DeleteForwarding.Run,
            ["DeleteResourceRecord"] = DeleteResourceRecord.Run,
            ["DeleteResourceRecordSet"] = DeleteResourceRecordSet.Run,
            ["DisableTheftProtectionLock"] = DisableTheftProtectionLock.Run,
            ["EditResourceRecord"] = EditResourceRecord.Run,
            ["EnableTheftProtectionLock"] = EnableTheftProtectionLock.Run,
            ["GetContacts"] = GetContacts.Run,
            ["GetCurrentBalance"] = GetCurrentBalance.Run,
            ["GetDetails"] = GetDetails.Run,
            ["GetForwarding"] = GetForwarding.Run,
            ["GetList"] = GetList.Run,
            ["GetResellerDetails"] = GetResellerDetails.Run,
            ["GetResourceRecords"] = GetResourceRecords.Run,
            ["GetTldList"] = GetTldList.Run,
            ["IsTrTLD"] = IsTrTLD.Run,
            ["ModifyChildNameServer"] = ModifyChildNameServer.Run,
            ["ModifyNameServer"] = ModifyNameServer.Run,
            ["ModifyPrivacyProtectionStatus"] = ModifyPrivacyProtectionStatus.Run,
            ["RegisterWithContactInfo"] = RegisterWithContactInfo.Run,
            ["RegisterWithContactInfo-TR"] = RegisterWithContactInfoTR.Run,
            ["RejectTransfer"] = RejectTransfer.Run,
            ["RemoveResourceRecordValue"] = RemoveResourceRecordValue.Run,
            ["Renew"] = Renew.Run,
            ["ReplaceResourceRecordValue"] = ReplaceResourceRecordValue.Run,
            ["SaveContacts"] = SaveContacts.Run,
            ["SetForwarding"] = SetForwarding.Run,
            ["SetResourceRecordSet"] = SetResourceRecordSet.Run,
            ["SyncFromRegistry"] = SyncFromRegistry.Run,
            ["Transfer"] = Transfer.Run,
            ["ValidateContact"] = ValidateContact.Run,
        };

        private static int Main(string[] args)
        {
            if (args.Length == 0 || !Examples.TryGetValue(args[0], out var example))
            {
                Console.WriteLine("Usage: dotnet run --project examples/DomainNameApi.Examples -- <Example>");
                Console.WriteLine();
                Console.WriteLine("Examples:");
                foreach (var name in Examples.Keys) Console.WriteLine("  " + name);
                return args.Length == 0 ? 0 : 1;
            }

            var resellerId = Environment.GetEnvironmentVariable("DNA_RESELLER_ID") ?? ResellerId;
            var apiKey = Environment.GetEnvironmentVariable("DNA_API_KEY") ?? ApiKey;
            var testMode = Environment.GetEnvironmentVariable("DNA_TEST_MODE") ?? "false";

            using (var dna = new DomainNameApiClient(resellerId, apiKey, testMode))
            {
                var result = example(dna);
                Console.WriteLine(result?.ToJsonString(new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                }) ?? "null");
            }
            return 0;
        }
    }
}
