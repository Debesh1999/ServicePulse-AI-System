using Microsoft.EntityFrameworkCore;
using ServicePulse.Infrastructure.Persistence;

namespace ServicePulse.Tests.TestHelpers;

/// <summary>
/// Creates an isolated in-memory SQLite database for controller tests.
/// Each test gets a fresh database with a unique name so tests run in parallel safely.
/// </summary>
public static class TestDbFactory
{
    public static ServicePulseDbContext CreateContext(string? name = null)
    {
        var suffix = name ?? Guid.NewGuid().ToString("N")[..8];
        var dbName = $"ServicePulseTest_{suffix}";

        var options = new DbContextOptionsBuilder<ServicePulseDbContext>()
            .UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=true;TrustServerCertificate=true")
            .Options;

        var context = new ServicePulseDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
