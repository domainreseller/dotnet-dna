using System.Reflection;
using System.Text.RegularExpressions;
using DomainNameApi.Accounts;
using DomainNameApi.Contacts;
using DomainNameApi.Dns;
using DomainNameApi.Dnssec;
using DomainNameApi.Domains;
using DomainNameApi.Hosting;
using DomainNameApi.Orders;
using DomainNameApi.Products;
using DomainNameApi.Ssl;

namespace DomainNameApi.Tests.Core;

/// <summary>
/// Keeps the SDK in step with the API: every operation in the OpenAPI document (testdata/openapi.json, the
/// same document the dna CLI embeds) has exactly one request class, with the CLI's safety class.
/// When the API adds an operation, this test fails until a request class exists.
/// </summary>
public class SpecCoverageTests
{
    private static readonly ContactInfo Contact = new();
    private static readonly Guid CertificateId = Guid.Parse("7d9f1a52-3c1e-4c9e-9a51-6b8c2f3e4d10");

    // One sample of every request, with the safety class the dna CLI assigns to the operation.
    private static readonly (BaseRequest Request, OperationSafety Safety)[] Requests =
    [
        (new GetAccountRequest(), OperationSafety.Read),
        (new GetProfileRequest(), OperationSafety.Read),
        (new ListTransactionsRequest(), OperationSafety.Read),

        (new GetContactRequest("DNA-1"), OperationSafety.Read),
        (new CreateContactRequest(Contact), OperationSafety.Write),
        (new UpdateContactRequest("DNA-1", Contact), OperationSafety.Write),
        (new GetContactVerificationRequest("DNA-1"), OperationSafety.Read),
        (new ResendContactVerificationRequest("DNA-1"), OperationSafety.Write),

        (new SearchDomainRequest("example.com"), OperationSafety.Read),
        (new BulkSearchDomainsRequest(["example.com"]), OperationSafety.Read),
        (new ListDomainsRequest(), OperationSafety.Read),
        (new GetDomainRequest("example.com"), OperationSafety.Read),
        (new RegisterDomainRequest("example.com"), OperationSafety.Billable),
        (new RegisterDomainWithHandlesRequest("example.com"), OperationSafety.Billable),
        (new RenewDomainRequest("example.com"), OperationSafety.Billable),
        (new CheckRenewalRequest("example.com"), OperationSafety.Read),
        (new TransferDomainRequest("example.com", "code"), OperationSafety.Billable),
        (new CheckTransferRequest("example.com", "code"), OperationSafety.Read),
        (new GetTransferStatusRequest("example.com"), OperationSafety.Read),
        (new LockDomainRequest("example.com"), OperationSafety.Write),
        (new UnlockDomainRequest("example.com"), OperationSafety.Write),
        (new SetPrivacyProtectionRequest("example.com", true), OperationSafety.Write),
        (new RestoreDomainRequest("example.com"), OperationSafety.Billable),
        (new UpdateAuthCodeRequest("example.com"), OperationSafety.Write),
        (new UpdateDomainContactsRequest("example.com", []), OperationSafety.Write),
        (new SetDomainContactHandlesRequest("example.com", []), OperationSafety.Write),
        (new SetNameServersRequest("example.com", []), OperationSafety.Write),
        (new GetForwardingRequest("example.com"), OperationSafety.Read),
        (new SetForwardingRequest("example.com", "https://example.net"), OperationSafety.Write),
        (new DeleteForwardingRequest("example.com"), OperationSafety.Destructive),
        (new AddChildNameServerRequest("example.com", "ns1.example.com", []), OperationSafety.Write),
        (new UpdateChildNameServerRequest("example.com", "ns1.example.com", []), OperationSafety.Write),
        (new DeleteChildNameServerRequest("example.com", "ns1.example.com"), OperationSafety.Destructive),
        (new ListDocumentsRequest("example.com"), OperationSafety.Read),
        (new UploadDocumentsRequest("example.com", []), OperationSafety.Write),

        (new ListDnsRecordsRequest("example.com"), OperationSafety.Read),
        (new AddDnsRecordRequest("example.com", "www", RecordType.A, ["192.0.2.1"]), OperationSafety.Write),
        (new ReplaceDnsRecordSetRequest("example.com", "www", RecordType.A, ["192.0.2.1"]), OperationSafety.Write),
        (new DeleteDnsRecordRequest("example.com", "www", RecordType.A, "192.0.2.1"), OperationSafety.Destructive),
        (new ApplyDnsZoneRequest("example.com"), OperationSafety.Write),

        (new GetDnssecRequest("example.com"), OperationSafety.Read),
        (new AddDsRecordsRequest("example.com", []), OperationSafety.Write),
        (new RemoveDsRecordRequest("example.com", new DsRecordData()), OperationSafety.Destructive),
        (new DisableDnssecRequest("example.com"), OperationSafety.Destructive),

        (new GenerateCsrRequest("example.com"), OperationSafety.Read),
        (new DecodeCsrRequest("csr"), OperationSafety.Read),
        (new PurchaseCertificateRequest("example.com", "PKG"), OperationSafety.Billable),
        (new RenewCertificateRequest(CertificateId), OperationSafety.Billable),
        (new ListCertificatesRequest(), OperationSafety.Read),
        (new ConfigureCertificateRequest(CertificateId, "csr", new CertificateContact()), OperationSafety.Write),
        (new RefreshCertificateStatusRequest(CertificateId), OperationSafety.Read),
        (new GetCertificateRequest(CertificateId), OperationSafety.Read),
        (new GetCertificateContentRequest(CertificateId), OperationSafety.Read),
        (new ReissueCertificateRequest(CertificateId), OperationSafety.Write),
        (new GetApproverEmailsRequest(CertificateId), OperationSafety.Read),
        (new RevokeCertificateRequest(CertificateId), OperationSafety.Destructive),

        (new ListHostingPackagesRequest(), OperationSafety.Read),
        (new CreateHostingRequest("example.com", "plan"), OperationSafety.Billable),
        (GetHostingRequest.ByDomain("example.com"), OperationSafety.Read),
        (new GetHostingByTokenRequest("token"), OperationSafety.Read),
        (GetHostingStatusRequest.ByDomain("example.com"), OperationSafety.Read),
        (new ListHostingPlansRequest(), OperationSafety.Read),

        (new GetOrderRequest("ORD-1"), OperationSafety.Read),

        (new ListProductsRequest(), OperationSafety.Read),
        (new GetProductPriceRequest("com", OrderType.Register), OperationSafety.Read),
        (new ListTldsRequest(), OperationSafety.Read),
        (new ListSslProductsRequest(), OperationSafety.Read),
    ];

    // The certificate requests with every optional value set, so that everything they can send is compared
    // with the API document. Existing certificates are addressed by ID; a domain name is only sent where the
    // document still has one (the list filter and the approvers of a domain without a certificate).
    private static readonly BaseRequest[] CertificateRequests =
    [
        new ListCertificatesRequest { DomainName = "example.com", SkipCount = 0, MaxResultCount = 10 },
        new RenewCertificateRequest(CertificateId),
        new ConfigureCertificateRequest(CertificateId, "csr", new CertificateContact { FirstName = "Ada" })
        {
            PrivateKey = "key",
            ValidationMethod = ValidationMethod.Email,
            ApproverEmail = "admin@example.com",
            WebServerType = WebServerType.Apache,
            SanList = "www.example.com",
            WildcardList = "*.example.com",
        },
        new RefreshCertificateStatusRequest(CertificateId),
        new GetCertificateRequest(CertificateId),
        new GetCertificateContentRequest(CertificateId),
        new ReissueCertificateRequest(CertificateId) { Csr = "csr", PrivateKey = "key", ApproverEmail = "admin@example.com" },
        new GetApproverEmailsRequest(CertificateId),
        new GetApproverEmailsRequest("example.com", "PKG"),
        new RevokeCertificateRequest(CertificateId),
    ];

    [Fact]
    public void CertificateRequestsSendOnlyWhatTheApiDocumentDescribes()
    {
        using var document = ReadSpec();
        var root = document.RootElement;

        foreach (var request in CertificateRequests)
        {
            var operation = root.GetProperty("paths").GetProperty("/api/v1/" + request.Path).GetProperty(request.Method.Method.ToLowerInvariant());

            var queryParameters = operation.TryGetProperty("parameters", out var parameters)
                ? parameters.EnumerateArray().Where(p => p.GetProperty("in").GetString() == "query").ToList()
                : [];
            var query = new QueryParameters();
            request.AddQueryParameters(query);
            var sentQuery = query.Select(p => p.Key).ToList();
            Assert.True(!sentQuery.Except(queryParameters.Select(p => p.GetProperty("name").GetString()!)).Any(),
                request + " sends query parameters the API does not have: " + string.Join(", ", sentQuery));
            Assert.Empty(queryParameters
                .Where(p => p.TryGetProperty("required", out var required) && required.GetBoolean())
                .Select(p => p.GetProperty("name").GetString()!)
                .Except(sentQuery));

            var body = request.GetBody();
            if (!operation.TryGetProperty("requestBody", out var requestBody))
            {
                Assert.True(body is null, request + " has no body in the API document");
                continue;
            }
            var reference = requestBody.GetProperty("content").EnumerateObject().First().Value.GetProperty("schema").GetProperty("$ref").GetString()!;
            var schema = root.GetProperty("components").GetProperty("schemas").GetProperty(reference.Substring("#/components/schemas/".Length));
            var sentBody = JsonSerializer.SerializeToElement(body!, body!.GetType(), DomainNameApi.Internal.Json.Options).Names();
            Assert.True(!sentBody.Except(schema.GetProperty("properties").Names()).Any(),
                request + " sends body properties the API does not have: " + string.Join(", ", sentBody));
            Assert.Empty(schema.GetProperty("required").EnumerateArray().Select(r => r.GetString()!).Except(sentBody));
        }
    }

    [Fact]
    public void EveryApiOperationHasExactlyOneRequest()
    {
        var spec = SpecOperations();
        var covered = Requests.Select(r => Key(r.Request)).ToList();

        Assert.Empty(spec.Keys.Except(covered));      // operations without a request
        Assert.Empty(covered.Except(spec.Keys));      // requests for operations the API does not have
        Assert.Equal(covered.Count, covered.Distinct().Count());
    }

    [Fact]
    public void EveryRequestClassIsInTheTable()
    {
        var requestTypes = typeof(BaseRequest).Assembly.GetTypes()
            .Where(t => !t.IsAbstract && typeof(BaseRequest).IsAssignableFrom(t))
            .Select(t => t.FullName)
            .OrderBy(n => n)
            .ToList();
        var tableTypes = Requests.Select(r => r.Request.GetType().FullName).OrderBy(n => n).ToList();

        Assert.Equal(requestTypes, tableTypes);
    }

    [Fact]
    public void SafetyMatchesTheCli()
    {
        foreach (var (request, safety) in Requests)
        {
            Assert.True(request.Safety == safety, request + " should be " + safety + ", not " + request.Safety);
        }
    }

    [Fact]
    public void ReadsAreIdempotent()
    {
        foreach (var (request, _) in Requests.Where(r => r.Safety == OperationSafety.Read))
        {
            Assert.True(request.IsIdempotent, request + " is a read and should be retried");
        }
        foreach (var (request, _) in Requests.Where(r => r.Safety == OperationSafety.Billable))
        {
            Assert.False(request.IsIdempotent, request + " charges money and must never be retried");
        }
    }

    private static string Key(BaseRequest request)
    {
        // contacts/DNA-1 -> contacts/{}, order/ORD-1 -> order/{}
        var path = Regex.Replace(request.Path, "^(contacts|order)/(?!verification).+$", "$1/{}");
        return request.Method.Method + " " + path;
    }

    private static JsonDocument ReadSpec() =>
        JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "testdata", "openapi.json")));

    private static Dictionary<string, string> SpecOperations()
    {
        using var document = ReadSpec();
        var result = new Dictionary<string, string>();
        foreach (var path in document.RootElement.GetProperty("paths").EnumerateObject())
        {
            foreach (var operation in path.Value.EnumerateObject())
            {
                if (operation.Name is "get" or "post" or "put" or "delete")
                {
                    var relative = Regex.Replace(path.Name.Substring("/api/v1/".Length), "\\{[^}]+\\}", "{}");
                    result[operation.Name.ToUpperInvariant() + " " + relative] = operation.Value.TryGetProperty("operationId", out var id) ? id.GetString() ?? "" : "";
                }
            }
        }
        return result;
    }
}
