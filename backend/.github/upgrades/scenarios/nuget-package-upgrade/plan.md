# NuGet Package Upgrade Plan

## Overview

**Target**: Remediate vulnerable and outdated NuGet dependencies in the backend targeting .NET 10.
**Scope**: Five specified backend projects; package edits will be limited to projects with the affected direct or transitive dependency references. The frontend SpaProxy project is audited separately and excluded from changes.

## Tasks

### 01-update-backend-dependencies: Update backend package references

Update the scoped EF Core packages from 10.0.0 to 10.0.15, keeping `Microsoft.EntityFrameworkCore.Tools` on the EF Core 10 major line to match Runtime/Design. Update the test collector and SDK (`coverlet.collector` 6.0.4 → 10.1.0, `Microsoft.NET.Test.Sdk` 17.14.1 → 18.11.1) and the Visual Studio xUnit runner (3.1.4 → 4.0.1); retain `xunit` 2.9.3 because the assessment resolved no newer version. Add private .NET 10 package overrides for the vulnerable transitive `System.Security.Cryptography.Pkcs` 9.0.4 and `System.Security.Cryptography.Xml` 9.0.0 dependencies, using 10.0.15 and placing them only in projects whose restored dependency graphs require them. The quick API diff reports three removals on internal EF Core types, so use build results to identify any actual source compatibility issue.

**Done when**: Scoped package references are updated consistently, package overrides resolve to patched versions, and modified projects restore/build without errors or warnings.

### 02-verify-backend-upgrade: Verify restore, vulnerability status, build, and tests

Restore the exact five scoped projects from the configured NuGet source, query NuGet vulnerability data for direct and transitive dependencies, build the backend solution/projects, and run the test project. The frontend remains excluded; its audit result is documented in the assessment.

**Done when**: Restore and builds pass, tests pass, and the scoped NuGet audit reports no remaining vulnerable packages. Any command that cannot run or fails is reported with its actual result.
