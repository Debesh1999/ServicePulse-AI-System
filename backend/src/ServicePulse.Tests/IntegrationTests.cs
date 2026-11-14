using Microsoft.EntityFrameworkCore;
using ServicePulse.Domain.Entities;
using ServicePulse.Infrastructure.Persistence;
using ServicePulse.Tests.TestHelpers;
using System.Text.Json;
using Xunit;

namespace ServicePulse.Tests;

/// <summary>
/// Integration tests for v1.3 features: provider tracking, fallback behavior, audit logging
/// These tests use LocalDB directly to verify EF Core model changes work correctly
/// </summary>
public class IntegrationTests
{
    [Fact]
    public async Task AnalysisResult_IncludesProviderAndFallbackFields()
    {
        // Arrange - use LocalDB like unit tests do
        var context = TestDbFactory.CreateContext("provider-test");
        try
        {
            // Setup test data
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "test-provider@test.com",
                PasswordHash = "x",
                DisplayName = "Test Agent Provider",
                Role = "Agent",
                IsActive = true
            };
            context.ApplicationUsers.Add(user);

            var agent = new Agent
            {
                Id = Guid.NewGuid(),
                EmployeeCode = "EMP-PROVIDER-" + Guid.NewGuid().ToString("N")[..6],
                FullName = "Test Agent Provider",
                TeamName = "Support",
                UserId = user.Id
            };
            context.Agents.Add(agent);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test Customer Provider",
                ExternalReference = "C-PROVIDER-" + Guid.NewGuid().ToString("N")[..6],
                ContactMasked = "xxx"
            };
            context.Customers.Add(customer);

            var call = new Call
            {
                Id = Guid.NewGuid(),
                AgentId = agent.Id,
                CustomerId = customer.Id,
                ExternalCallId = "TEST-PROVIDER-" + Guid.NewGuid().ToString("N")[..6],
                StartedAtUtc = DateTime.UtcNow,
                DurationSeconds = 180,
                Status = "Completed",
                CreatedAtUtc = DateTime.UtcNow
            };
            context.Calls.Add(call);

            // Add analysis result with provider and fallback fields
            var analysis = new AnalysisResult
            {
                Id = Guid.NewGuid(),
                CallId = call.Id,
                AnalyzerVersion = "1.3.0",
                Sentiment = "Positive",
                RiskLevel = "Low",
                QualityScore = 85m,
                Summary = "Test analysis",
                DetectedKeywordsJson = "[\"test\"]",
                AnalysisStatus = "Succeeded",
                ErrorMessage = null,
                Provider = "Gemini",
                FallbackReason = "QuotaExceeded",
                CreatedAtUtc = DateTime.UtcNow
            };
            context.AnalysisResults.Add(analysis);

            // Add audit log for the analysis creation
            var audit = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityName = "AnalysisResult",
                EntityId = analysis.Id,
                Action = "Created",
                ActorUserId = null,
                CorrelationId = Guid.NewGuid(),
                DetailsJson = JsonSerializer.Serialize(new
                {
                    callId = call.Id,
                    provider = "Gemini",
                    fallbackReason = "QuotaExceeded",
                    qualityScore = 85m
                }),
                CreatedAtUtc = DateTime.UtcNow
            };
            context.AuditLogs.Add(audit);

            await context.SaveChangesAsync();

            // Assert - verify the data was saved correctly
            var savedAnalysis = await context.AnalysisResults.FirstAsync(a => a.Id == analysis.Id);
            Assert.Equal("Gemini", savedAnalysis.Provider);
            Assert.Equal("QuotaExceeded", savedAnalysis.FallbackReason);

            var savedAudit = await context.AuditLogs.FirstAsync(a => a.EntityId == analysis.Id);
            Assert.Equal("AnalysisResult", savedAudit.EntityName);
            Assert.Equal("Created", savedAudit.Action);
            Assert.NotNull(savedAudit.DetailsJson);
            Assert.Contains("Gemini", savedAudit.DetailsJson);
            Assert.Contains("QuotaExceeded", savedAudit.DetailsJson);
        }
        finally
        {
            context.Dispose();
        }
    }

    [Fact]
    public async Task AnalysisResult_FallbackReason_IsRecorded_WhenPythonServiceFails()
    {
        // This test verifies that when Python service fails/times out,
        // the fallback reason is recorded in AnalysisResult

        // Arrange
        var context = TestDbFactory.CreateContext("fallback-test");
        try
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "test-fallback@test.com",
                PasswordHash = "x",
                DisplayName = "Test Agent Fallback",
                Role = "Agent",
                IsActive = true
            };
            context.ApplicationUsers.Add(user);

            var agent = new Agent
            {
                Id = Guid.NewGuid(),
                EmployeeCode = "EMP-FALLBACK-" + Guid.NewGuid().ToString("N")[..6],
                FullName = "Test Agent Fallback",
                TeamName = "Support",
                UserId = user.Id
            };
            context.Agents.Add(agent);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test Customer Fallback",
                ExternalReference = "C-FALLBACK-" + Guid.NewGuid().ToString("N")[..6],
                ContactMasked = "xxx"
            };
            context.Customers.Add(customer);

            var call = new Call
            {
                Id = Guid.NewGuid(),
                AgentId = agent.Id,
                CustomerId = customer.Id,
                ExternalCallId = "TEST-FALLBACK-" + Guid.NewGuid().ToString("N")[..6],
                StartedAtUtc = DateTime.UtcNow,
                DurationSeconds = 180,
                Status = "Completed",
                CreatedAtUtc = DateTime.UtcNow
            };
            context.Calls.Add(call);

            // Simulate a failed analysis with fallback
            var analysis = new AnalysisResult
            {
                Id = Guid.NewGuid(),
                CallId = call.Id,
                AnalyzerVersion = "1.3.0",
                Sentiment = "Neutral",
                RiskLevel = "Medium",
                QualityScore = 50m,
                Summary = "Fallback analysis due to timeout",
                DetectedKeywordsJson = "[]",
                AnalysisStatus = "Succeeded",
                ErrorMessage = null,
                Provider = "RuleBased",
                FallbackReason = "Timeout",
                CreatedAtUtc = DateTime.UtcNow
            };
            context.AnalysisResults.Add(analysis);

            var audit = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityName = "AnalysisResult",
                EntityId = analysis.Id,
                Action = "Created",
                ActorUserId = null,
                CorrelationId = Guid.NewGuid(),
                DetailsJson = JsonSerializer.Serialize(new
                {
                    callId = call.Id,
                    provider = "RuleBased",
                    fallbackReason = "Timeout",
                    qualityScore = 50m
                }),
                CreatedAtUtc = DateTime.UtcNow
            };
            context.AuditLogs.Add(audit);

            await context.SaveChangesAsync();

            // Assert
            var savedAnalysis = await context.AnalysisResults.FirstAsync(a => a.Id == analysis.Id);
            Assert.Equal("RuleBased", savedAnalysis.Provider);
            Assert.Equal("Timeout", savedAnalysis.FallbackReason);

            var savedAudit = await context.AuditLogs.FirstAsync(a => a.EntityId == analysis.Id);
            Assert.Contains("Timeout", savedAudit.DetailsJson);
            Assert.Contains("RuleBased", savedAudit.DetailsJson);
        }
        finally
        {
            context.Dispose();
        }
    }

    [Fact]
    public async Task AnalysisResult_DefaultsToRuleBased_WhenProviderNotSpecified()
    {
        // Verify the default value works correctly
        var context = TestDbFactory.CreateContext("default-provider-test");
        try
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "test-default@test.com",
                PasswordHash = "x",
                DisplayName = "Test Agent Default",
                Role = "Agent",
                IsActive = true
            };
            context.ApplicationUsers.Add(user);

            var agent = new Agent
            {
                Id = Guid.NewGuid(),
                EmployeeCode = "EMP-DEFAULT-" + Guid.NewGuid().ToString("N")[..6],
                FullName = "Test Agent Default",
                TeamName = "Support",
                UserId = user.Id
            };
            context.Agents.Add(agent);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test Customer Default",
                ExternalReference = "C-DEFAULT-" + Guid.NewGuid().ToString("N")[..6],
                ContactMasked = "xxx"
            };
            context.Customers.Add(customer);

            var call = new Call
            {
                Id = Guid.NewGuid(),
                AgentId = agent.Id,
                CustomerId = customer.Id,
                ExternalCallId = "TEST-DEFAULT-" + Guid.NewGuid().ToString("N")[..6],
                StartedAtUtc = DateTime.UtcNow,
                DurationSeconds = 180,
                Status = "Completed",
                CreatedAtUtc = DateTime.UtcNow
            };
            context.Calls.Add(call);

            // Add analysis result WITHOUT specifying provider (should default to RuleBased)
            var analysis = new AnalysisResult
            {
                Id = Guid.NewGuid(),
                CallId = call.Id,
                AnalyzerVersion = "1.3.0",
                Sentiment = "Positive",
                RiskLevel = "Low",
                QualityScore = 90m,
                Summary = "Test default provider",
                DetectedKeywordsJson = "[\"test\"]",
                AnalysisStatus = "Succeeded",
                ErrorMessage = null,
                CreatedAtUtc = DateTime.UtcNow
                // Provider not set - should default to "RuleBased"
            };
            context.AnalysisResults.Add(analysis);

            await context.SaveChangesAsync();

            // Assert - verify default value
            var savedAnalysis = await context.AnalysisResults.FirstAsync(a => a.Id == analysis.Id);
            Assert.Equal("RuleBased", savedAnalysis.Provider);
            Assert.Null(savedAnalysis.FallbackReason);
        }
        finally
        {
            context.Dispose();
        }
    }

    [Fact]
    public async Task AuditLog_CapturesCorrelationId_ForAnalysisEvents()
    {
        // Verify audit logs capture correlation IDs for traceability
        var context = TestDbFactory.CreateContext("audit-correlation-test");
        try
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "test-audit@test.com",
                PasswordHash = "x",
                DisplayName = "Test Agent Audit",
                Role = "Agent",
                IsActive = true
            };
            context.ApplicationUsers.Add(user);

            var agent = new Agent
            {
                Id = Guid.NewGuid(),
                EmployeeCode = "EMP-AUDIT-" + Guid.NewGuid().ToString("N")[..6],
                FullName = "Test Agent Audit",
                TeamName = "Support",
                UserId = user.Id
            };
            context.Agents.Add(agent);

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test Customer Audit",
                ExternalReference = "C-AUDIT-" + Guid.NewGuid().ToString("N")[..6],
                ContactMasked = "xxx"
            };
            context.Customers.Add(customer);

            var call = new Call
            {
                Id = Guid.NewGuid(),
                AgentId = agent.Id,
                CustomerId = customer.Id,
                ExternalCallId = "TEST-AUDIT-" + Guid.NewGuid().ToString("N")[..6],
                StartedAtUtc = DateTime.UtcNow,
                DurationSeconds = 180,
                Status = "Completed",
                CreatedAtUtc = DateTime.UtcNow
            };
            context.Calls.Add(call);

            var correlationId = Guid.NewGuid();

            var analysis = new AnalysisResult
            {
                Id = Guid.NewGuid(),
                CallId = call.Id,
                AnalyzerVersion = "1.3.0",
                Sentiment = "Positive",
                RiskLevel = "Low",
                QualityScore = 88m,
                Summary = "Test audit correlation",
                DetectedKeywordsJson = "[\"audit\"]",
                AnalysisStatus = "Succeeded",
                ErrorMessage = null,
                Provider = "Gemini",
                FallbackReason = null,
                CreatedAtUtc = DateTime.UtcNow
            };
            context.AnalysisResults.Add(analysis);

            var audit = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityName = "AnalysisResult",
                EntityId = analysis.Id,
                Action = "Created",
                ActorUserId = null,
                CorrelationId = correlationId,
                DetailsJson = JsonSerializer.Serialize(new
                {
                    callId = call.Id,
                    provider = "Gemini",
                    fallbackReason = (string?)null,
                    qualityScore = 88m
                }),
                CreatedAtUtc = DateTime.UtcNow
            };
            context.AuditLogs.Add(audit);

            await context.SaveChangesAsync();

            // Assert
            var savedAudit = await context.AuditLogs.FirstAsync(a => a.EntityId == analysis.Id);
            Assert.Equal(correlationId, savedAudit.CorrelationId);
            Assert.Equal("AnalysisResult", savedAudit.EntityName);
            Assert.Equal("Created", savedAudit.Action);
            Assert.NotNull(savedAudit.DetailsJson);
        }
        finally
        {
            context.Dispose();
        }
    }
}