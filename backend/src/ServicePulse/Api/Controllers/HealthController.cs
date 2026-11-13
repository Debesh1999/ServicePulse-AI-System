using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServicePulse.Infrastructure.Persistence;

namespace ServicePulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ServicePulseDbContext _context;
    private readonly IHostEnvironment _env;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        ServicePulseDbContext context,
        IHostEnvironment env,
        ILogger<HealthController> logger)
    {
        _context = context;
        _env = env;
        _logger = logger;
    }

    /// <summary>
    /// Returns service health including database connectivity.
    /// Returns 200 when healthy, 503 when the database is unreachable.
    /// </summary>
    [HttpGet]
    [Produces(typeof(HealthStatus))]
    [EndpointName("health")]
    [EndpointDescription("Service health check")]
    public async Task<IActionResult> GetHealth()
    {
        bool dbConnected;
        try
        {
            dbConnected = await _context.Database.CanConnectAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Health check: database connectivity probe failed");
            dbConnected = false;
        }

        var health = new HealthStatus(
            dbConnected ? "Healthy" : "Unhealthy",
            _env.EnvironmentName,
            dbConnected ? "Connected" : "Disconnected",
            DateTime.UtcNow
        );

        if (!dbConnected)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, health);
        }

        return Ok(health);
    }
}

public record HealthStatus(
    string Status,
    string Environment,
    string Database,
    DateTime Timestamp
);