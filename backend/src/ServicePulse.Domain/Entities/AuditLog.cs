using System.ComponentModel.DataAnnotations;

namespace ServicePulse.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string EntityName { get; set; } = string.Empty; // Call, AnalysisResult, ManagerReview

    [Required]
    public Guid EntityId { get; set; }

    [Required]
    [MaxLength(80)]
    public string Action { get; set; } = string.Empty; // Created, Queued, OverrideRecorded

    public Guid? ActorUserId { get; set; } // Null for background jobs

    public Guid? CorrelationId { get; set; }

    public string? DetailsJson { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual ApplicationUser? ActorUser { get; set; }
}