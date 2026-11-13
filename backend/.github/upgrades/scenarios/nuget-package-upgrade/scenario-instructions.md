# NuGet Package Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Scope**: Assess exactly the five requested backend projects: `src/ServicePulse.Api/ServicePulse.Api.csproj`, `src/ServicePulse.Application/ServicePulse.Application.csproj`, `src/ServicePulse.Domain/ServicePulse.Domain.csproj`, `src/ServicePulse.Infrastructure/ServicePulse.Infrastructure.csproj`, and `src/ServicePulse.Tests/ServicePulse.Tests.csproj`.
- **Packages**: Assess `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`, `coverlet.collector`, `Microsoft.NET.Test.Sdk`, `xunit`, and `xunit.runner.visualstudio`.
- **Version Policy**: Latest stable versions supported by the scoped projects targeting .NET 10.
- **Frontend Scope**: Inspect `Microsoft.AspNetCore.SpaProxy` separately; do not modify the frontend project as part of this backend upgrade.
- **Execution**: Minimize unrelated changes; restore, build, and run tests where possible. Report validation only when commands actually pass.
- **Execution Style**: Keep work narrowly focused on the scoped NuGet fixes and required validation; avoid unnecessary investigation, changes, or verbose progress reporting.
- **Commit Strategy**: Manual (the workspace directory is not a Git repository, so no commits will be made).

## Decisions
- Confirmed the specified project scope and latest-stable version policy before initialization.
- The backend directory is not detected as a Git repository; no branch setup applies.
- NuGet audit found additional transitive vulnerabilities in `System.Security.Cryptography.Pkcs` 9.0.4 and `System.Security.Cryptography.Xml` 9.0.0 within the scoped project dependency graph; remediate these in scope with .NET 10 package versions.
- Keep `Microsoft.EntityFrameworkCore.Tools` on the EF Core 10.0.x line to align with the runtime/design packages rather than accepting the assessment's newer 11.0.0 major recommendation.
