# 01-update-backend-dependencies: Update backend package references

Update the scoped EF Core packages from 10.0.0 to 10.0.15, keeping `Microsoft.EntityFrameworkCore.Tools` on the EF Core 10 major line to match Runtime/Design. Update the test collector and SDK (`coverlet.collector` 6.0.4 → 10.1.0, `Microsoft.NET.Test.Sdk` 17.14.1 → 18.11.1) and the Visual Studio xUnit runner (3.1.4 → 4.0.1); retain `xunit` 2.9.3 because the assessment resolved no newer version. Add private .NET 10 package overrides for the vulnerable transitive `System.Security.Cryptography.Pkcs` 9.0.4 and `System.Security.Cryptography.Xml` 9.0.0 dependencies, using 10.0.15 and placing them only in projects whose restored dependency graphs require them. The quick API diff reports three removals on internal EF Core types, so use build results to identify any actual source compatibility issue.

## Scope Inventory

- **Projects affected**: `ServicePulse.Api`, `ServicePulse.Domain`, `ServicePulse.Infrastructure`, and `ServicePulse.Tests` have direct package references or vulnerable transitive dependencies to update. `ServicePulse.Application` targets `net10.0` and has no direct package references; no project-file change is needed there.
- **Distinct concerns**: (1) EF Core 10 runtime/design/tools alignment; (2) test tooling versions; (3) .NET cryptography transitive vulnerability overrides.
- **Package management**: `get_project_dependencies` confirms package references are defined directly in each `.csproj`; no central package management is present. `NuGet.Config` points to nuget.org. All five requested projects target `net10.0`.
- **Direct package locations and changes**:
  - `src/ServicePulse.Api/ServicePulse.Api.csproj`: `Microsoft.EntityFrameworkCore.Design` 10.0.0 → 10.0.15; NuGet audit also finds Pkcs 9.0.4 transitively.
  - `src/ServicePulse.Domain/ServicePulse.Domain.csproj`: `Microsoft.EntityFrameworkCore` 10.0.0 → 10.0.15.
  - `src/ServicePulse.Infrastructure/ServicePulse.Infrastructure.csproj`: SQL Server and Tools 10.0.0 → 10.0.15; NuGet audit finds Pkcs 9.0.4 and Xml 9.0.0 transitively.
  - `src/ServicePulse.Tests/ServicePulse.Tests.csproj`: collector 6.0.4 → 10.1.0, test SDK 17.14.1 → 18.11.1, xUnit runner 3.1.4 → 4.0.1; keep xunit 2.9.3; its restore graph also reports Pkcs 9.0.4.
- **Dependency paths**: `dotnet nuget why` traces Pkcs/Xml through the EF Core Tools → Design graph in Infrastructure and through EF Core Design/build tooling in Api. Add Pkcs 10.0.15 as a private override in the projects where audit reports it (Api and Infrastructure); add Pkcs and Xml 10.0.15 as private overrides in Infrastructure. Build/test project audit results will verify propagation.
- **Audit evidence**: NuGet restore reports Pkcs 9.0.4 with one high and one moderate advisory, and Xml 9.0.0 with eleven high advisory entries. Current advisory ranges include Pkcs and Xml 10.0.x versions through 10.0.12 as affected; 10.0.15 is outside those ranges. The package assessment reports three removed EF Core members in internal namespaces and no other API removals; quick mode did not locate source call sites.
- **Breakdown assessment**: the scope is bounded to one package-reference update across four `.csproj` files, with no central version file, framework migration, or code migration anticipated. Keep this as one atomic implementation task and follow it with a separate final audit/build/test task.

**Done when**: Scoped package references are updated consistently, package overrides resolve to patched versions, and modified projects restore/build without errors or warnings.
