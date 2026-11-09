# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-11-09

### Added

- First release of the .NET client for the DomainNameAPI reseller REST API, a faithful port of php-dna 4.0.4.
- `DomainNameApiClient` with all 45 php-dna methods: account and balance, availability search, domain and TLD lists,
  domain details and sync, contacts, nameservers, registrar lock, child nameservers, privacy, renewal, registration
  (including .tr TRABIS attributes), transfers (start, cancel, approve, reject, check), DNS zone management
  (record and record-set level, with read-merge-write helpers) and URL forwarding.
- Return values as `System.Text.Json.Nodes.JsonNode` with exactly the structure, key names, string flags and
  formatted amounts of php-dna; errors are returned as an envelope, never thrown.
- Contact completion and normalisation (`ValidateContact`), host name normalisation, .tr attribute normalisation.
- HTTP layer on `HttpClient` with the `DomainNameAPI-DotNet/1.0.0` User-Agent, 30 s / 10 s timeouts, optional TLS
  verification, no redirect following, zone call pacing and a replaceable `IHttpTransport`.
- Error mapping identical to php-dna: `API_<status>` codes, synthesised messages for empty bodies and ABP
  boilerplate, flattened validation errors, `API_<errno>` for timeouts and other network failures.
- Background Sentry telemetry (errors and sampled performance) with credential masking, minimal mode and an
  on/off switch; the DSN is empty in this release, so nothing is sent yet.
- Targets .NET Standard 2.0 and .NET 8 with no third-party runtime dependencies.
- Runnable examples for every method, including the DNS zone and forwarding methods.
- xUnit test suite including all 126 php-dna conformance fixtures.
- README in English, Turkish, German, French, Italian, Dutch, Azerbaijani, Arabic and Simplified Chinese.

[1.0.0]: https://github.com/domainreseller/dotnet-dna/releases/tag/v1.0.0
