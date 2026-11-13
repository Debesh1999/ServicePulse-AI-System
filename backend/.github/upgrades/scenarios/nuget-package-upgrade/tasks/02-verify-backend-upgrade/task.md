# 02-verify-backend-upgrade: Verify restore, vulnerability status, build, and tests

Restore the exact five scoped projects from the configured NuGet source, query NuGet vulnerability data for direct and transitive dependencies, build the backend solution/projects, and run the test project. The frontend remains excluded; its audit result is documented in the assessment.

**Done when**: Restore and builds pass, tests pass, and the scoped NuGet audit reports no remaining vulnerable packages. Any command that cannot run or fails is reported with its actual result.
