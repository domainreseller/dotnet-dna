# AGENTS.md

Guidance for people and AI coding agents working on this repository.

## Commands

```bash
dotnet build                 # library, tests and examples
dotnet test                  # net10.0, plus net48 on Windows
dotnet pack src/DomainNameApi/DomainNameApi.csproj -c Release
```

## Layout

- `src/DomainNameApi/DomainNameApiClient.cs`: the entry point. Sending, retries, pacing, error handling.
- `src/DomainNameApi/Core/`: `BaseRequest`, `PagedRequest`, `QueryParameters`, `DomainNameApiException`, responses, shared enums.
- `src/DomainNameApi/Internal/`: JSON settings, error parsing, the rate-limit pacer. Not public.
- One folder per API area (`Accounts`, `Contacts`, `Domains`, `Dns`, `Dnssec`, `Ssl`, `Hosting`, `Orders`, `Products`), each with
  `XxxClient.cs`, `Requests/` (one class per operation) and `Models/`. All types of an area share one namespace.
- `tests/DomainNameApi.Tests/testdata/openapi.json`: the API document, the same one the `dna` CLI embeds.

## Rules

- **New API operation:** update `testdata/openapi.json` from the CLI repository, add a request class in the area's
  `Requests/` folder, models in `Models/`, a method on the area client, and a line in `SpecCoverageTests.Requests`.
  `SpecCoverageTests` fails until all of this is done.
- **Safety class** (`OperationSafety`) is a product decision and must match the CLI's registry:
  `Billable` for anything that charges the balance, `Destructive` for deletes and hard-to-undo changes, `Write` for free,
  reversible changes, `Read` only for operations without side effects (even when the HTTP method is POST or PUT).
- **Never retry billable requests** after network errors or 5xx answers. Only 429 is safe to retry for them.
- **Models are forgiving:** response properties are nullable or have defaults; requests validate before sending.
- **Tests never call the real API.** Use `TestClient` and `FakeHttpHandler`.
- **Public API:** every public member has an XML doc comment. Error codes, `ApiErrorKind` values and safety classes are a contract.
- **README translations:** `README.md` is the source; keep the eight `README-XX.md` translations in sync.
- **Style:** English for code, comments and docs; user-facing messages say what happened and what to do next.
- **Commits:** imperative subject line. Do not add `Co-Authored-By` trailers for AI tools.
