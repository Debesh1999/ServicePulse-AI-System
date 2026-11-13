# Progress Details

Updated the EF Core packages in `ServicePulse.Api`, `ServicePulse.Domain`, and `ServicePulse.Infrastructure` from 10.0.0 to 10.0.15. Added private `System.Security.Cryptography.Pkcs` 10.0.15 overrides in Api and Infrastructure and a private `System.Security.Cryptography.Xml` 10.0.15 override in Infrastructure to address the NuGet audit findings. Kept EF Tools on 10.0.15 to align with the EF Core 10 family.

Validation: `dotnet build src/ServicePulse.Tests/ServicePulse.Tests.csproj --verbosity minimal` passed with no warnings or errors and built the dependent scoped projects. `dotnet test src/ServicePulse.Tests/ServicePulse.Tests.csproj --verbosity minimal` passed (1 test, 0 failed). The package references are direct per-project references; no CPM was detected.

Resolved the unrelated API build lock reported during the task by stopping the existing `ServicePulse.Api` process (PID 31768); the subsequent build passed. No source code changes were needed for the internal EF Core API diff.