using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServicePulse.Domain.Enums;

namespace ServicePulse.Domain.Entities;

public class Call
{
    public Guid Id { get; set; }

    [Required]
    public Guid AgentId { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [MaxLength(100)]
    public string? ExternalCallId { get; set; }

    [Required]
    public DateTime StartedAtUtc { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int DurationSeconds { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Queued"; // Draft, Queued, Analyzing, Completed, Failed

    [MaxLength(30)]
    public string? ResolutionStatus { get; set; } // Open, Resolved, Escalated, Unknown

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public byte[]? RowVersion { get; set; }

    // Navigation
    public virtual Agent Agent { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<CallTranscript> Transcripts { get; set; } = new List<CallTranscript>();
    public virtual ICollection<AnalysisResult> AnalysisResults { get; set; } = new List<AnalysisResult>();
    public virtual ICollection<ManagerReview> ManagerReviews { get; set; } = new List<ManagerReview>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}