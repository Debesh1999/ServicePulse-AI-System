using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServicePulse.Application.Services;
using ServicePulse.Domain.Entities;
using ServicePulse.Infrastructure.Persistence;

namespace ServicePulse.Infrastructure.BackgroundJobs;

public static class CallAnalysisJob
{
    public static void QueueAnalysis(Guid callId)
    {
        // Only queue if Hangfire is available (not in test environment)
        if (ServiceLocator.Instance != null)
        {
            BackgroundJob.Enqueue(() => ProcessAnalysisAsync(callId));
        }
    }

    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 30, 120, 300 })]
    public static async Task ProcessAnalysisAsync(Guid callId)
    {
        // We need to create a new scope to get scoped services
        using var scope = CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ServicePulseDbContext>();
        var aiService = scope.ServiceProvider.GetRequiredService<IAiAnalyzerService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CallAnalysisJobRunner>>();

        try
        {
            logger.LogInformation("Starting AI analysis for call {CallId}", callId);

            var call = await context.Calls
                .Include(c => c.Transcripts)
                .FirstOrDefaultAsync(c => c.Id == callId);

            if (call == null)
            {
                logger.LogWarning("Call {CallId} not found", callId);
                return;
            }

            var transcript = call.Transcripts
                .OrderByDescending(t => t.CreatedAtUtc)
                .FirstOrDefault();

            if (transcript == null)
            {
                logger.LogWarning("No transcript found for call {CallId}", callId);
                return;
            }

            // Update status to Analyzing
            call.Status = "Analyzing";
            await context.SaveChangesAsync();

            // Run AI analysis
            var aiResult = await aiService.AnalyzeAsync(transcript.Text, transcript.LanguageCode);

            // Save analysis result with provider and fallback info
            var analysis = new AnalysisResult
            {
                Id = aiResult.Id,
                CallId = call.Id,
                AnalyzerVersion = aiResult.AnalyzerVersion,
                Sentiment = aiResult.Sentiment,
                RiskLevel = aiResult.RiskLevel,
                QualityScore = aiResult.QualityScore,
                Summary = aiResult.Summary,
                DetectedKeywordsJson = System.Text.Json.JsonSerializer.Serialize(aiResult.Keywords),
                AnalysisStatus = aiResult.AnalysisStatus,
                ErrorMessage = aiResult.ErrorMessage,
                Provider = aiResult.Provider,
                FallbackReason = aiResult.FallbackReason,
                CreatedAtUtc = DateTime.UtcNow
            };

            context.AnalysisResults.Add(analysis);

            // Write audit log for AI analysis completion with correlation ID
            var correlationId = Guid.NewGuid();
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityName = "AnalysisResult",
                EntityId = analysis.Id,
                Action = "Created",
                ActorUserId = null, // Background job
                CorrelationId = correlationId,
                DetailsJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    callId = callId,
                    provider = aiResult.Provider,
                    fallbackReason = aiResult.FallbackReason,
                    qualityScore = aiResult.QualityScore,
                    sentiment = aiResult.Sentiment,
                    riskLevel = aiResult.RiskLevel,
                    analyzerVersion = aiResult.AnalyzerVersion
                }),
                CreatedAtUtc = DateTime.UtcNow
            };
            context.AuditLogs.Add(auditLog);

            // Update call status
            call.Status = aiResult.AnalysisStatus == "Succeeded" ? "Completed" : "Failed";

            await context.SaveChangesAsync();

            logger.LogInformation("AI analysis completed for call {CallId}: {Status}, Provider={Provider}, FallbackReason={FallbackReason}",
                callId, aiResult.AnalysisStatus, aiResult.Provider, aiResult.FallbackReason ?? "none");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process AI analysis for call {CallId}", callId);

            // Write audit log for failure
            var correlationId = Guid.NewGuid();
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityName = "AnalysisResult",
                EntityId = Guid.Empty,
                Action = "Failed",
                ActorUserId = null,
                CorrelationId = correlationId,
                DetailsJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    callId = callId,
                    error = ex.Message
                }),
                CreatedAtUtc = DateTime.UtcNow
            };
            context.AuditLogs.Add(auditLog);

            // Update call status to Failed
            var call = await context.Calls.FindAsync(callId);
            if (call != null)
            {
                call.Status = "Failed";
                await context.SaveChangesAsync();
            }
        }
    }

    private static IServiceScope CreateScope()
    {
        var serviceProvider = ServiceLocator.Instance;
        return serviceProvider.CreateScope();
    }
}

/// <summary>
/// Simple service locator to access Hangfire's job activator
/// In production, use Hangfire's built-in DI integration via IBackgroundJobClient
/// </summary>
public static class ServiceLocator
{
    public static IServiceProvider Instance { get; set; } = null!;
}

/// <summary>
/// Dummy class for logger generic type parameter
/// </summary>
internal class CallAnalysisJobRunner { }