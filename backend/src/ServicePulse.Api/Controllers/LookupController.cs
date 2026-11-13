using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicePulse.Infrastructure.Persistence;

namespace ServicePulse.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LookupController : ControllerBase
{
    private readonly ServicePulseDbContext _context;

    public LookupController(ServicePulseDbContext context)
    {
        _context = context;
    }

    [HttpGet("agents")]
    public async Task<IActionResult> GetAgents()
    {
        var agents = await _context.Agents
            .OrderBy(a => a.FullName)
            .Select(a => new { id = a.Id.ToString(), name = a.FullName })
            .ToListAsync();
        return Ok(agents);
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _context.Customers
            .OrderBy(c => c.DisplayName)
            .Select(c => new { id = c.Id.ToString(), name = c.DisplayName })
            .ToListAsync();
        return Ok(customers);
    }
}
