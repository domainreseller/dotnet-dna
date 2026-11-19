# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - Unreleased

A new SDK, modelled on the [`dna` CLI](https://github.com/domainreseller/cli) and built from the same API document.
1.x was a port of the PHP library; 2.0 is not compatible with it. See "Upgrading from 1.x" in the README.

### Added

- Typed clients for all 64 operations of the REST API: `Account`, `Contacts`, `Domains` (with `Forwarding`,
  `ChildNameServers` and `Documents`), `Dns`, `Dnssec`, `Ssl`, `Hosting`, `Orders` and `Products`.
  DNSSEC, SSL certificates, hosting, orders, products, contact handles, registry documents, auth codes, restores and
  the transaction ledger are new.
- One request class per operation, all derived from `BaseRequest`, `BaseRequest<TResponse>` or `PagedRequest<TItem>`.
  Subclass them for endpoints the SDK does not cover; `DomainNameApiClient.SendAsync` sends any request.
- Typed models with documentation for every answer, enums for coded values (`DomainStatus`, `TransactionType`,
  `DnssecAlgorithm`, ...) and forgiving text enums (`RecordType`, `ContactType`, `ForwardType`, ...).
- Async methods with `CancellationToken` everywhere; `ListAllAsync` methods page automatically through `IAsyncEnumerable`.
- Every request declares an `OperationSafety` (`Read`, `Write`, `Billable`, `Destructive`), the same classes the CLI uses.
  `ReadOnly` mode blocks everything but reads before it is sent.
- Retries like the CLI: 429 always (honouring `Retry-After`), network errors and 502/503/504 only for reads, so a
  billable request is never sent twice. Client-side pacing keeps requests under the gateway's rate limits.
- `DomainNameApiException` with a stable `Kind`, the API's error `Code`, validation errors, correlation ID, hint,
  `IsRetryable` and `OutcomeUnknown`.
- `SendWithResponseAsync` returns the correlation ID, the soft-quota state, the number of attempts and the duration.
- `DomainNameApiClient.FromEnvironment()` reads the CLI's variables (`DNA_RESELLER_ID`, `DNA_API_KEY`, `DNA_ENV`,
  `DNA_TEST_MODE`, `DNA_BASE_URL`, `DNA_READ_ONLY`).
- Works as a typed `HttpClient` with `IHttpClientFactory`; the client is thread-safe.
- Checks before sending: missing fields, incomplete contacts, missing contact roles, invalid IP addresses, CNAME at the
  apex, CAA values and TTL ranges throw `ArgumentException` without calling the API.
- Helpers: `DomainContact.ForAllRoles`, `ContactHandle.ForAllRoles`, `HostIpAddress.Parse`, `DsRecordData.Parse`,
  `DocumentUpload.FromFile`, `CertificateContent.FullChain`, `Tld.GetRegistrationPrice`, `ProductsClient.GetTldAsync`.
- xUnit tests with a fake HTTP handler, run on .NET 10 and .NET Framework 4.8, including a check that every operation
  in the API document has a request class with the CLI's safety class.

### Changed

- Targets .NET Standard 2.0 and .NET 10 (the .NET 8 target is replaced; .NET 8 and 9 apps use the .NET Standard 2.0 build).
- The `System.Text.Json` dependency of the .NET Standard 2.0 build is updated to 10.0.15.
- Nullable reference types are enabled; shared build settings live in `Directory.Build.props`.
- CI runs on Linux and Windows; the package includes a symbol package.

### Removed

- The php-dna compatible API (`JsonNode` envelopes, `GetRequestData`, `SetError` and the other PHP-style helpers) and
  the php-dna conformance fixtures.
- Sentry error reporting and the public IP lookup. The SDK sends nothing anywhere except to the API.

## [1.0.1] - 2026-11-14

### Changed

- Error reporting now has its own Sentry project: `DefaultSentryDsn` points to DomainNameAPI's dotnet-dna project, so
  telemetry is active by default as documented (in 1.0.0 the DSN was empty and nothing was sent).
- Turn it off with `SetErrorReportingEnabled(false)`; the `TelemetryDsn` option still overrides the destination.
- Version bumped to 1.0.1 (User-Agent `DomainNameAPI-DotNet/1.0.1`).

## [1.0.0] - Unreleased

### Added

- First release of the .NET client for the DomainNameAPI reseller REST API, a faithful port of php-dna (as of commit f946d69).
- `DomainNameApiClient` with all 45 php-dna methods: account and balance, availability search, domain and TLD lists,
  domain details and sync, contacts, nameservers, registrar lock, child nameservers, privacy, renewal, registration
  (including .tr TRABIS attributes), transfers (start, cancel, approve, reject, check), DNS zone management
  (record and record-set level, with read-merge-write helpers) and URL forwarding.
- Return values as `System.Text.Json.Nodes.JsonNode` with exactly the structure, key names, string flags and
  formatted amounts of php-dna; errors are returned as an envelope, never thrown.
- Contact completion and normalisation (`ValidateContact`: missing state falls back to the city, numbers keep their
  own `+`/`00` country code), case-insensitive contact types in `GetContacts`, host name normalisation, .tr attribute
  normalisation.
- HTTP layer on `HttpClient` with the `DomainNameAPI-DotNet/1.0.0` User-Agent, 30 s / 10 s timeouts, optional TLS
  verification, no redirect following, zone call pacing and a replaceable `IHttpTransport`.
- Error mapping identical to php-dna: `API_<status>` codes for HTTP errors with the gateway's own code as
  `GatewayCode`, synthesised messages for empty bodies and ABP boilerplate (never the raw response body),
  flattened validation errors, `CURL_<errno>` for timeouts and other network failures.
- Refusals are errors, not successes: registration, renewal and transfer answered with `"success": false`
  return `REGISTER_DOMAIN` / `RENEW_DOMAIN` / `TRANSFER_DOMAIN`; a failed automatic zone apply returns the apply
  error; `GetCurrentBalance` fails with `BALANCE` for currencies the account does not report.
- Transfers never send contacts (the registrar takes them over); `Transfer` has no contacts parameter.
- URL forwarding: only "Redirect information not found" means "not configured"; `DeleteForwarding` reports
  `Removed` and is a no-op success when nothing is configured.
- `ModifyPrivacyProtectionStatus` also accepts the flag as text ("1", "true", "on", "yes"); the reason is not sent.
- Background Sentry telemetry (errors and sampled performance) with credential masking, minimal mode and an
  on/off switch; the DSN is empty in this release, so nothing is sent yet.
- Targets .NET Standard 2.0 and .NET 8 with no third-party runtime dependencies.
- Runnable examples for every method, including the DNS zone and forwarding methods.
- xUnit test suite including all 134 php-dna conformance fixtures.
- README in English, Turkish, German, French, Italian, Dutch, Azerbaijani, Arabic and Simplified Chinese.

[2.0.0]: https://github.com/domainreseller/dotnet-dna/releases/tag/v2.0.0
[1.0.1]: https://github.com/domainreseller/dotnet-dna/releases/tag/v1.0.1
[1.0.0]: https://github.com/domainreseller/dotnet-dna/releases/tag/v1.0.0
