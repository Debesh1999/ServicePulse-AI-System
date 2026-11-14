using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using ServicePulse.Application.DTOs;
using ServicePulse.Application.Services;
using ServicePulse.Domain.Entities;
using ServicePulse.Infrastructure.BackgroundJobs;
using ServicePulse.Infrastructure.Persistence;

namespace ServicePulse.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CallsController : ControllerBase
{
    private readonly ServicePulseDbContext _context;
    private readonly IAiAnalyzerService _aiService;
    private readonly ILogger<CallsController> _logger;

    public CallsController(ServicePulseDbContext context, IAiAnalyzerService aiService, ILogger<CallsController> logger)
    {
        _context = context;
        _aiService = aiService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCalls([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var calls = await _context.Calls
            .Include(c => c.Agent)
            .Include(c => c.Customer)
            .OrderByDescending(c => c.StartedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CallDto(
                c.Id,
                c.Agent.FullName,
                c.Customer.DisplayName,
                c.StartedAtUtc,
                c.DurationSeconds,
                c.Status
            ))
            .ToListAsync();

        return Ok(calls);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCall(Guid id)
    {
        var call = await _context.Calls
            .Include(c => c.Agent)
            .Include(c => c.Customer)
            .Include(c => c.Transcripts)
            .Include(c => c.AnalysisResults)
            .Include(c => c.ManagerReviews)
                .ThenInclude(r => r.ReviewerUser)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (call == null)
        {
            return NotFound();
        }

        var transcript = call.Transcripts.OrderByDescending(t => t.CreatedAtUtc).FirstOrDefault();
        var analysis = call.AnalysisResults
            .OrderByDescending(a => a.CreatedAtUtc)
            .FirstOrDefault();
        var review = call.ManagerReviews
            .OrderByDescending(r => r.ReviewedAtUtc)
            .FirstOrDefault();

        var analysisDto = analysis == null
            ? null
            : new AnalysisResultDto(
                analysis.Id,
                analysis.AnalyzerVersion,
                analysis.Sentiment,
                analysis.RiskLevel,
                analysis.QualityScore,
                analysis.Summary,
                ParseKeywords(analysis.DetectedKeywordsJson),
                analysis.AnalysisStatus,
                analysis.ErrorMessage,
                analysis.Provider,
                analysis.FallbackReason
            );

        var reviewDto = review == null
            ? null
            : new ManagerReviewDto(
                review.Id,
                review.ReviewerUser.DisplayName,
                review.ReviewStatus,
                review.ManagerScore,
                review.CoachingNotes,
                review.OverrideReason,
                review.ReviewedAtUtc
            );

        var dto = new CallDetailDto(
            call.Id,
            call.Agent.FullName,
            call.Customer.DisplayName,
            call.ExternalCallId,
            call.StartedAtUtc,
            call.DurationSeconds,
            call.Status,
            transcript?.Text,
            analysisDto,
            reviewDto
        );

        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchCalls(
        [FromQuery] string? agentName,
        [FromQuery] string? customerName,
        [FromQuery] string? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Calls
            .Include(c => c.Agent)
            .Include(c => c.Customer)
            .AsQueryable();

        if (!string.IsNullOrEmpty(agentName))
        {
            query = query.Where(c => c.Agent.FullName.Contains(agentName));
        }

        if (!string.IsNullOrEmpty(customerName))
        {
            query = query.Where(c => c.Customer.DisplayName.Contains(customerName));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(c => c.Status == status);
        }

        if (from.HasValue)
        {
            query = query.Where(c => c.StartedAtUtc >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(c => c.StartedAtUtc <= to.Value);
        }

        var totalCount = await query.CountAsync();

        var calls = await query
            .OrderByDescending(c => c.StartedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CallDto(
                c.Id,
                c.Agent.FullName,
                c.Customer.DisplayName,
                c.StartedAtUtc,
                c.DurationSeconds,
                c.Status
            ))
            .ToListAsync();

        return Ok(new PagedCallListDto(calls, totalCount, page, pageSize));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalCalls = await _context.Calls.CountAsync();
        var completedCalls = await _context.Calls.CountAsync(c => c.Status == "Completed");
        var analyzingCalls = await _context.Calls.CountAsync(c => c.Status == "Analyzing");
        var failedCalls = await _context.Calls.CountAsync(c => c.Status == "Failed");
        var queuedCalls = await _context.Calls.CountAsync(c => c.Status == "Queued");

        var avgDurationDouble = await _context.Calls
            .Where(c => c.DurationSeconds > 0)
            .Select(c => (double?)c.DurationSeconds)
            .DefaultIfEmpty()
            .AverageAsync();
        var avgDuration = avgDurationDouble.HasValue ? (int)Math.Round(avgDurationDouble.Value) : 0;

        var statusCounts = await _context.Calls
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count);

        var agentCounts = await _context.Calls
            .Include(c => c.Agent)
            .GroupBy(c => c.Agent.FullName)
            .Select(g => new { AgentName = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.AgentName, g => g.Count);

        var customerCounts = await _context.Calls
            .Include(c => c.Customer)
            .GroupBy(c => c.Customer.DisplayName)
            .Select(g => new { CustomerName = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CustomerName, g => g.Count);

        var stats = new
        {
            TotalCalls = totalCalls,
            CompletedCalls = completedCalls,
            AnalyzingCalls = analyzingCalls,
            FailedCalls = failedCalls,
            QueuedCalls = queuedCalls,
            AverageDurationSeconds = avgDuration,
            StatusCounts = statusCounts,
            AgentCounts = agentCounts,
            CustomerCounts = customerCounts
        };

        return Ok(stats);
    }

    private static List<string> ParseKeywords(string? keywordsJson)
    {
        if (string.IsNullOrEmpty(keywordsJson))
        {
            return new List<string>();
        }

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(keywordsJson)
                ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCall([FromBody] CreateCallRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AgentName))
            return BadRequest(new { message = "Agent name is required." });
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            return BadRequest(new { message = "Customer name is required." });
        if (request.TranscriptText == null || request.TranscriptText.Length < 1)
            return BadRequest(new { message = "Transcript text is required and cannot be empty." });
        if (request.DurationSeconds < 1 || request.DurationSeconds > 7200)
            return BadRequest(new { message = "Duration must be between 1 and 7200 seconds." });
        if (request.ExternalCallId != null && request.ExternalCallId.Length > 50)
            return BadRequest(new { message = "External call ID must be 50 characters or less." });

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Look up Agent and Customer by name
            var agent = await _context.Agents
                .FirstOrDefaultAsync(a => a.FullName == request.AgentName);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.DisplayName == request.CustomerName);

            if (agent == null || customer == null)
            {
                return BadRequest(new { message = "Agent or Customer not found." });
            }

            var call = new Call
            {
                Id = Guid.NewGuid(),
                AgentId = agent.Id,
                CustomerId = customer.Id,
                ExternalCallId = request.ExternalCallId,
                StartedAtUtc = request.StartedAtUtc,
                DurationSeconds = request.DurationSeconds,
                Status = "Queued",
                CreatedAtUtc = DateTime.UtcNow
            };

            var transcript = new CallTranscript
            {
                Id = Guid.NewGuid(),
                CallId = call.Id,
                Text = request.TranscriptText,
                Source = "ManualUpload",
                LanguageCode = "en",
                CreatedAtUtc = DateTime.UtcNow,
                WordCount = request.TranscriptText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length
            };

            _context.Calls.Add(call);
            _context.CallTranscripts.Add(transcript);

            await _context.SaveChangesAsync();

            // Queue the AI analysis as a background job via Hangfire
            // This is done before committing so if Hangfire fails, the call isn't created
            CallAnalysisJob.QueueAnalysis(call.Id);

            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetCall), new { id = call.Id }, new CallDetailDto(
                call.Id,
                agent.FullName,
                customer.DisplayName,
                call.ExternalCallId,
                call.StartedAtUtc,
                call.DurationSeconds,
                call.Status,
                transcript.Text,
                null,
                null
            ));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to create call");
            return StatusCode(500, "An error occurred while creating the call.");
        }
    }
}