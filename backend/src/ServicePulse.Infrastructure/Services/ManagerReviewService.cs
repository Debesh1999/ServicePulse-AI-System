using Microsoft.EntityFrameworkCore;
using ServicePulse.Application.DTOs;
using ServicePulse.Application.Services;
using ServicePulse.Domain.Entities;
using ServicePulse.Infrastructure.Persistence;

namespace ServicePulse.Infrastructure.Services;

public sealed class ManagerReviewService : IManagerReviewService
{
    private readonly ServicePulseDbContext _context;

    public ManagerReviewService(ServicePulseDbContext context)
    {
        _context = context;
    }

    public async Task<ManagerReviewDto> CreateReviewAsync(Guid reviewerUserId, CreateManagerReviewRequest request)
    {
        var call = await _context.Calls.FindAsync(request.CallId);
        if (call == null)
            throw new ArgumentException("Call not found", nameof(request.CallId));

        var reviewer = await _context.ApplicationUsers.FindAsync(reviewerUserId);
        if (reviewer == null)
            throw new ArgumentException("Reviewer not found", nameof(reviewerUserId));

        var review = new ManagerReview
        {
            Id = Guid.NewGuid(),
            CallId = request.CallId,
            ReviewerUserId = reviewerUserId,
            ReviewStatus = request.ReviewStatus,
            ManagerScore = request.ManagerScore,
            CoachingNotes = request.CoachingNotes,
            OverrideReason = request.OverrideReason,
            ReviewedAtUtc = DateTime.UtcNow
        };

        _context.ManagerReviews.Add(review);

        // Create audit log
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityName = "ManagerReview",
            EntityId = review.Id,
            Action = "ReviewCreated",
            ActorUserId = reviewerUserId,
            CorrelationId = Guid.NewGuid(),
            DetailsJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                callId = request.CallId,
                reviewStatus = request.ReviewStatus,
                managerScore = request.ManagerScore,
                overrideReason = request.OverrideReason
            }),
            CreatedAtUtc = DateTime.UtcNow
        };
        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();

        return new ManagerReviewDto(
            review.Id,
            reviewer.DisplayName,
            review.ReviewStatus,
            review.ManagerScore,
            review.CoachingNotes,
            review.OverrideReason,
            review.ReviewedAtUtc
        );
    }

    public async Task<ManagerReviewDto> UpdateReviewAsync(Guid reviewId, Guid reviewerUserId, UpdateManagerReviewRequest request)
    {
        var review = await _context.ManagerReviews
            .Include(r => r.ReviewerUser)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review == null)
            throw new ArgumentException("Review not found", nameof(reviewId));

        if (review.ReviewerUserId != reviewerUserId)
            throw new UnauthorizedAccessException("You can only update your own reviews");

        review.ReviewStatus = request.ReviewStatus;
        review.ManagerScore = request.ManagerScore;
        review.CoachingNotes = request.CoachingNotes;
        review.OverrideReason = request.OverrideReason;
        review.ReviewedAtUtc = DateTime.UtcNow;

        // Create audit log
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityName = "ManagerReview",
            EntityId = review.Id,
            Action = "ReviewUpdated",
            ActorUserId = reviewerUserId,
            CorrelationId = Guid.NewGuid(),
            DetailsJson = System.Text.Json.JsonSerializer.Serialize(new
            {
                reviewStatus = request.ReviewStatus,
                managerScore = request.ManagerScore,
                overrideReason = request.OverrideReason
            }),
            CreatedAtUtc = DateTime.UtcNow
        };
        _context.AuditLogs.Add(auditLog);

        await _context.SaveChangesAsync();

        return new ManagerReviewDto(
            review.Id,
            review.ReviewerUser.DisplayName,
            review.ReviewStatus,
            review.ManagerScore,
            review.CoachingNotes,
            review.OverrideReason,
            review.ReviewedAtUtc
        );
    }

    public async Task<ManagerReviewDto?> GetReviewAsync(Guid reviewId)
    {
        var review = await _context.ManagerReviews
            .Include(r => r.ReviewerUser)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review == null)
            return null;

        return new ManagerReviewDto(
            review.Id,
            review.ReviewerUser.DisplayName,
            review.ReviewStatus,
            review.ManagerScore,
            review.CoachingNotes,
            review.OverrideReason,
            review.ReviewedAtUtc
        );
    }

    public async Task<List<ManagerReviewDto>> GetReviewsForCallAsync(Guid callId)
    {
        var reviews = await _context.ManagerReviews
            .Include(r => r.ReviewerUser)
            .Where(r => r.CallId == callId)
            .OrderByDescending(r => r.ReviewedAtUtc)
            .ToListAsync();

        return reviews.Select(r => new ManagerReviewDto(
            r.Id,
            r.ReviewerUser.DisplayName,
            r.ReviewStatus,
            r.ManagerScore,
            r.CoachingNotes,
            r.OverrideReason,
            r.ReviewedAtUtc
        )).ToList();
    }

    public async Task<ManagerReviewDto> GetLatestReviewForCallAsync(Guid callId)
    {
        var review = await _context.ManagerReviews
            .Include(r => r.ReviewerUser)
            .Where(r => r.CallId == callId)
            .OrderByDescending(r => r.ReviewedAtUtc)
            .FirstOrDefaultAsync();

        if (review == null)
            throw new ArgumentException("No review found for call", nameof(callId));

        return new ManagerReviewDto(
            review.Id,
            review.ReviewerUser.DisplayName,
            review.ReviewStatus,
            review.ManagerScore,
            review.CoachingNotes,
            review.OverrideReason,
            review.ReviewedAtUtc
        );
    }
}