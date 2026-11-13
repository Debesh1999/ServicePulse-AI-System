# 01.01-ef-data-layer: Update EF Core and cryptography dependencies

## Objective
Update the backend API, domain, and infrastructure package references to aligned, patched .NET 10 versions, including the transitive cryptography vulnerabilities found by NuGet Audit.

## Scope
- `src/ServicePulse.Api/ServicePulse.Api.csproj`: `Microsoft.EntityFrameworkCore.Design` 10.0.0 → 10.0.15 and private `System.Security.Cryptography.Pkcs` 10.0.15 override.
- `src/ServicePulse.Domain/ServicePulse.Domain.csproj`: `Microsoft.EntityFrameworkCore` 10.0.0 → 10.0.15.
- `src/ServicePulse.Infrastructure/ServicePulse.Infrastructure.csproj`: `Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.EntityFrameworkCore.Tools` 10.0.0 → 10.0.15; private overrides for `System.Security.Cryptography.Pkcs` and `System.Security.Cryptography.Xml` at 10.0.15.
- Keep Tools on EF Core major 10, not the assessment's newest 11.0.0, to preserve Design/runtime major alignment.

## Research Findings
- `get_project_dependencies` confirms all package versions are declared in the named project files; no CPM central version file applies.
- All projects involved target `net10.0`. `ServicePulse.Api` currently declares EF Design 10.0.0; `ServicePulse.Domain` declares EF Core 10.0.0; `ServicePulse.Infrastructure` declares EF SQL Server and Tools 10.0.0.
- NuGet Audit found Pkcs 9.0.4 (high and moderate advisories) in Api and Infrastructure; the test project's transitive graph also reports Pkcs. Infrastructure additionally resolves Xml 9.0.0 with eleven high advisory entries.
- `dotnet nuget why` traces these from EF Design/Tools build-time dependencies. The source projects mark the EF tooling references private; cryptography override references should also use `PrivateAssets="all"` to avoid changing downstream public dependencies.
- Current GitHub advisory ranges flag Pkcs 10.0.0–10.0.12 and Xml 10.0.0–10.0.12 in the latest advisories; 10.0.15 is outside those ranges. The package assessment recommends EF runtime/design/SQL Server 10.0.15. EF Tools 11.0.0 was not selected because it would mismatch the EF 10 family.
- The quick EF API diff lists three removals under internal namespaces only; the build will determine whether any project code is affected.

## Steps
1. Update the three project files as listed above.
2. Restore and build the affected projects; resolve any API/build issues.

**Done when**: All three projects restore/build without errors or warnings and their EF Core packages resolve to 10.0.15 with the patched private cryptography packages.
