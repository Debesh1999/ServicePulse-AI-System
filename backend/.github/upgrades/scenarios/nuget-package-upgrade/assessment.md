# NuGet package upgrade assessment

_Mode: **quick assessment** — package API diffs only; no per-project source scan was run._

## Recommended versions

- **coverlet.collector**: **10.1.0** (unified across 1 project(s)).
- **Microsoft.EntityFrameworkCore**: **10.0.15** (unified across 1 project(s)).
- **Microsoft.EntityFrameworkCore.Design**: **10.0.15** (unified across 1 project(s)).
- **Microsoft.EntityFrameworkCore.SqlServer**: **10.0.15** (unified across 1 project(s)).
- **Microsoft.EntityFrameworkCore.Tools**: **11.0.0** (unified across 1 project(s)).
- **Microsoft.NET.Test.Sdk**: **18.11.1** (unified across 1 project(s)).
- **System.Security.Cryptography.Pkcs**: not referenced by any scoped project.
- **System.Security.Cryptography.Xml**: not referenced by any scoped project.
- **xunit**: **2.9.3** (unified across 1 project(s)).
- **xunit.runner.visualstudio**: **4.0.1** (unified across 1 project(s)).

## Public API changes

> **Types moved (namespace changed) are not removals.** A moved type keeps its name and members;
> the fix is a `using`-directive change, not a rewrite. Do not treat a moved type as deleted.

- **Microsoft.EntityFrameworkCore**: 3 member(s) removed — see [`apidiff/Microsoft.EntityFrameworkCore.apidiff.md`](apidiff/Microsoft.EntityFrameworkCore.apidiff.md).
- **Microsoft.EntityFrameworkCore.SqlServer**: no source-breaking public API changes detected — see [`apidiff/Microsoft.EntityFrameworkCore.SqlServer.apidiff.md`](apidiff/Microsoft.EntityFrameworkCore.SqlServer.apidiff.md).
- **Microsoft.EntityFrameworkCore.Tools**: no source-breaking public API changes detected — see [`apidiff/Microsoft.EntityFrameworkCore.Tools.apidiff.md`](apidiff/Microsoft.EntityFrameworkCore.Tools.apidiff.md).
- **Microsoft.EntityFrameworkCore.Design**: no source-breaking public API changes detected — see [`apidiff/Microsoft.EntityFrameworkCore.Design.apidiff.md`](apidiff/Microsoft.EntityFrameworkCore.Design.apidiff.md).
- **coverlet.collector**: no source-breaking public API changes detected — see [`apidiff/coverlet.collector.apidiff.md`](apidiff/coverlet.collector.apidiff.md).
- **Microsoft.NET.Test.Sdk**: no source-breaking public API changes detected — see [`apidiff/Microsoft.NET.Test.Sdk.apidiff.md`](apidiff/Microsoft.NET.Test.Sdk.apidiff.md).
- **xunit.runner.visualstudio**: no source-breaking public API changes detected — see [`apidiff/xunit.runner.visualstudio.apidiff.md`](apidiff/xunit.runner.visualstudio.apidiff.md).

## Breaking-change findings

- Version divergence findings (Pkg.0003): 0
- Requested-version-unsupported findings (Pkg.0002): 0

- Quick mode does not scan source, so there are no per-line `PkgApi` usage findings. Review the
  per-package API diffs above and rely on build errors during execution to pinpoint affected code.
- A full code scan can locate the exact source location of every breaking-change usage across the repo.
  It is opt-in and slower — re-run the assessment with `fullScan=true` only if the user requests it.

## Next steps

1. Proceed to planning to triage the API changes above and plan the code fixes (if any).

## Supplemental NuGet vulnerability audit

The scoped projects target `net10.0`. After restoring the five requested projects from the configured NuGet source, `dotnet restore` / `dotnet list package --vulnerable --include-transitive` reported:

- `System.Security.Cryptography.Pkcs` 9.0.4 — transitive in `ServicePulse.Api`, `ServicePulse.Infrastructure`, and `ServicePulse.Tests`; NuGet reports one high and one moderate advisory (GHSA-846f-f764-76mx and GHSA-2fmv-rv2c-59x7).
- `System.Security.Cryptography.Xml` 9.0.0 — transitive in `ServicePulse.Infrastructure`; NuGet reports 11 high-severity advisory entries.
- Current GitHub advisory version ranges include affected 10.0.x releases through 10.0.12 for the latest Pkcs/Xml advisories. The proposed 10.0.15 versions are beyond those affected ranges.

These are transitive dependencies introduced by `Microsoft.EntityFrameworkCore.Design` / `Microsoft.EntityFrameworkCore.Tools` build-time dependency graphs, so they are not represented as direct package references in the generated package API assessment. Plan direct, private dependency overrides only where NuGet audit requires them. The separately inspected `frontend/ServicePulse.Frontend/ServicePulse.Frontend.csproj` uses `Microsoft.AspNetCore.SpaProxy` 7.0.19 and targets out-of-support `net7.0`; its package audit reported no vulnerable packages from the configured sources. The frontend is outside the confirmed backend scope and will not be modified.

