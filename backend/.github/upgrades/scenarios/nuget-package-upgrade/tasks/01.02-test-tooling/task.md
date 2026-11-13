# 01.02-test-tooling: Update test tooling packages

## Objective
Update only the test project’s outdated test tooling while preserving its existing xUnit framework version.

## Scope
- `src/ServicePulse.Tests/ServicePulse.Tests.csproj`: `coverlet.collector` 6.0.4 → 10.1.0, `Microsoft.NET.Test.Sdk` 17.14.1 → 18.11.1, and `xunit.runner.visualstudio` 3.1.4 → 4.0.1.
- Keep `xunit` at 2.9.3 because the current package assessment recommended no newer version.

## Steps
1. Update the three PackageReference versions.
2. Restore and build the test project, then run its tests.
3. Verify NuGet Audit resolves Pkcs from the updated backend dependency graph.

**Done when**: The test project restores and builds without errors or warnings, tests pass, and the intended tooling versions resolve.

