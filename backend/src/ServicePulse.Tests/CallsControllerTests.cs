using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicePulse.Api.Controllers;
using ServicePulse.Application.DTOs;
using ServicePulse.Domain.Entities;
using ServicePulse.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ServicePulse.Tests;

public class CallsControllerTests
{
    private static ControllerContext CreateCtx() =>
        new ControllerContext { HttpContext = new DefaultHttpContext(), RouteData = new Microsoft.AspNetCore.Routing.RouteData() };

    // Fresh DB file per test to ensure isolation
    private static string DbPath => Path.Combine(Path.GetTempPath(), "sp_test_" + Guid.NewGuid().ToString("N")[..8] + ".db");

    private static void SetupDb(ServicePulseDbContext ctx)
    {
        ctx.Database.EnsureDeleted();
        ctx.Database.OpenConnection();
        ctx.Database.EnsureCreated();
    }

    private static (ServicePulseDbContext, CallsController) NewCtx()
    {
        var opts = new DbContextOptionsBuilder<ServicePulseDbContext>()
            .UseSqlite($"Data Source={DbPath};Cache=Shared")
            .Options;
        var ctx = new ServicePulseDbContext(opts);
        SetupDb(ctx);
        return (ctx, new CallsController(ctx) { ControllerContext = CreateCtx() });
    }

    [Fact]
    public async Task GetCalls_ReturnsPaginatedResults()
    {
        var (ctx, ctrl) = Seed();
        var result = await ctrl.GetCalls(page: 1, pageSize: 10);
        var ok = Assert.IsType<OkObjectResult>(result);
        var calls = Assert.IsAssignableFrom<List<CallDto>>(ok.Value);
        Assert.Equal(3, calls.Count);
    }

    [Fact]
    public async Task GetCall_ReturnsNotFound_ForUnknownId()
    {
        var (_, ctrl) = Seed();
        Assert.IsType<NotFoundResult>(await ctrl.GetCall(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetCall_ReturnsAnalysisAndReview_WhenPresent()
    {
        var (_, ctrl, call) = SeedFull();
        var ok = Assert.IsType<OkObjectResult>(await ctrl.GetCall(call.Id));
        var dto = Assert.IsType<CallDetailDto>(ok.Value);
        Assert.NotNull(dto.Analysis);
        Assert.NotNull(dto.Review);
        Assert.Equal("Positive", dto.Analysis.Sentiment);
        Assert.Equal("Mgr One", dto.Review.ReviewerDisplayName);
    }

    [Fact]
    public async Task SearchCalls_ReturnsFilteredResults_ByAgentName()
    {
        var (_, ctrl) = Seed();
        var ok = Assert.IsType<OkObjectResult>(await ctrl.SearchCalls(agentName: "Alice", customerName: null, status: null, from: null, to: null));
        var paged = Assert.IsType<PagedCallListDto>(ok.Value);
        Assert.Equal(2, paged.Calls.Count);
        foreach (var c in paged.Calls)
            Assert.Equal("Alice", c.AgentName);
        Assert.Equal(2, paged.TotalCount);
    }

    [Fact]
    public async Task SearchCalls_ReturnsFilteredResults_ByStatus()
    {
        var (_, ctrl) = Seed();
        var ok = Assert.IsType<OkObjectResult>(await ctrl.SearchCalls(null, null, "Completed", null, null));
        var paged = Assert.IsType<PagedCallListDto>(ok.Value);
        Assert.Equal(2, paged.TotalCount);
        foreach (var c in paged.Calls)
            Assert.Equal("Completed", c.Status);
    }

    [Fact]
    public async Task GetStats_ReturnsCorrectCounts()
    {
        var (_, ctrl) = Seed();
        var result = await ctrl.GetStats();
        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonSerializer.Serialize(ok.Value);
        var stats = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(stats);
        Assert.Equal(3, stats["TotalCalls"].GetInt32());
        Assert.Equal(2, stats["CompletedCalls"].GetInt32());
        Assert.Equal(1, stats["QueuedCalls"].GetInt32());
        Assert.True(stats["AverageDurationSeconds"].GetInt32() > 0);
    }

    [Fact]
    public async Task CreateCall_ReturnsBadRequest_WhenAgentNotFound()
    {
        var (_, ctrl) = Seed();
        var req = new CreateCallRequest("Nobody", "Acme Corp", "T", DateTime.UtcNow, 60, "T");
        var bad = Assert.IsType<BadRequestObjectResult>(await ctrl.CreateCall(req));
        Assert.NotNull(bad.Value);
    }

    [Fact]
    public async Task CreateCall_CreatesCallSuccessfully()
    {
        var (ctx, ctrl) = Seed();
        var req = new CreateCallRequest("Alice", "Acme", "TEST-2", DateTime.UtcNow, 90, "Hello world transcript");
        var created = Assert.IsType<CreatedAtActionResult>(await ctrl.CreateCall(req));
        var dto = Assert.IsType<CallDetailDto>(created.Value);
        Assert.Equal("Queued", dto.Status);
        var saved = await ctx.Calls.FindAsync(dto.Id);
        Assert.NotNull(saved);
        Assert.Equal("Queued", saved.Status);
    }

    #region Helpers
    private static (ServicePulseDbContext, CallsController) Seed()
    {
        var (ctx, ctrl) = NewCtx();

        // Create the ApplicationUser row needed by Agent.UserId FK
        ctx.ApplicationUsers.Add(new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "agent@test.com",
            PasswordHash = "x",
            DisplayName = "Agent User",
            Role = "Agent",
            IsActive = true
        });
        ctx.SaveChanges();

        var a1 = AddAgent(ctx, "Alice");
        var a2 = AddAgent(ctx, "Bob");
        var c1 = AddCustomer(ctx, "Acme");
        var c2 = AddCustomer(ctx, "Globex");

        ctx.Calls.Add(new Call
        {
            Id = Guid.NewGuid(),
            AgentId = a1.Id, CustomerId = c1.Id,
            ExternalCallId = "X1",
            StartedAtUtc = DateTime.Parse("2026-01-01T10:00:00Z"),
            DurationSeconds = 120, Status = "Completed",
            CreatedAtUtc = DateTime.Parse("2026-01-01T10:05:00Z")
        });
        ctx.Calls.Add(new Call
        {
            Id = Guid.NewGuid(),
            AgentId = a2.Id, CustomerId = c2.Id,
            ExternalCallId = "X2",
            StartedAtUtc = DateTime.Parse("2026-01-01T10:00:00Z"),
            DurationSeconds = 300, Status = "Completed",
            CreatedAtUtc = DateTime.Parse("2026-01-01T10:05:00Z")
        });
        ctx.Calls.Add(new Call
        {
            Id = Guid.NewGuid(),
            AgentId = a1.Id, CustomerId = c1.Id,
            ExternalCallId = "X3",
            StartedAtUtc = DateTime.Parse("2026-01-01T10:00:00Z"),
            DurationSeconds = 60, Status = "Queued",
            CreatedAtUtc = DateTime.Parse("2026-01-01T10:05:00Z")
        });

        ctx.SaveChanges();
        return (ctx, ctrl);
    }

    private static (ServicePulseDbContext, CallsController, Call) SeedFull()
    {
        var (ctx, ctrl) = NewCtx();

        ctx.ApplicationUsers.Add(new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "agent@test.com",
            PasswordHash = "x",
            DisplayName = "Agent User",
            Role = "Agent",
            IsActive = true
        });
        ctx.SaveChanges();

        var a = AddAgent(ctx, "TestAgent");
        var c = AddCustomer(ctx, "TestCustomer");

        var call = new Call
        {
            Id = Guid.NewGuid(),
            AgentId = a.Id, CustomerId = c.Id,
            ExternalCallId = "XF",
            StartedAtUtc = DateTime.Parse("2026-01-01T10:00:00Z"),
            DurationSeconds = 180, Status = "Completed",
            CreatedAtUtc = DateTime.Parse("2026-01-01T10:05:00Z")
        };
        ctx.Calls.Add(call);
        ctx.SaveChanges(); // persist call first so FK to call id works

        ctx.AnalysisResults.Add(new AnalysisResult
        {
            Id = Guid.NewGuid(),
            CallId = call.Id,
            AnalyzerVersion = "1.0.0",
            Sentiment = "Positive",
            RiskLevel = "Low",
            QualityScore = 90m,
            Summary = "Test summary",
            DetectedKeywordsJson = "[\"billing\",\"refund\"]",
            AnalysisStatus = "Succeeded",
            ErrorMessage = null,
            CreatedAtUtc = DateTime.Parse("2026-01-01T11:00:00Z")
        });

        var reviewerId = Guid.NewGuid();
        ctx.ApplicationUsers.Add(new ApplicationUser
        {
            Id = reviewerId,
            Email = "mgr@test.com",
            PasswordHash = "h",
            DisplayName = "Mgr One",
            Role = "Manager",
            IsActive = true
        });
        ctx.ManagerReviews.Add(new ManagerReview
        {
            Id = Guid.NewGuid(),
            CallId = call.Id,
            ReviewerUserId = reviewerId,
            ReviewStatus = "Approved",
            ManagerScore = 95m,
            CoachingNotes = "Good",
            OverrideReason = null,
            ReviewedAtUtc = DateTime.UtcNow
        });
        ctx.SaveChanges();
        return (ctx, ctrl, call);
    }

    private static Agent AddAgent(ServicePulseDbContext ctx, string fullName)
    {
        var agent = new Agent
        {
            Id = Guid.NewGuid(),
            EmployeeCode = "EMP-" + Guid.NewGuid().ToString("N")[..6],
            FullName = fullName,
            TeamName = "Sales"
        };
        ctx.Agents.Add(agent);
        ctx.SaveChanges();
        return agent;
    }

    private static Customer AddCustomer(ServicePulseDbContext ctx, string name)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            DisplayName = name,
            ExternalReference = "C-" + Guid.NewGuid().ToString("N")[..6]
        };
        ctx.Customers.Add(customer);
        ctx.SaveChanges();
        return customer;
    }
    #endregion
}
