using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePulse.Domain.Entities;

public class ManagerReview
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string ReviewStatus { get; set; } = "Pending";

    [Range(0, 100)]
    [Column(TypeName = "decimal(5,2)")]
    public decimal? ManagerScore { get; set; }

    [MaxLength(2000)]
    public string? CoachingNotes { get; set; }

    [MaxLength(500)]
    public string? OverrideReason { get; set; }

    public DateTime ReviewedAtUtc { get; set; } = DateTime.UtcNow;

    // Explicit FK for EF Core - links to ApplicationUser who authored the review
    public Guid ReviewerUserId { get; set; }

    // Explicit FK for EF Core - links to Call
    public Guid CallId { get; set; }

    // Navigation
    public virtual Call Call { get; set; } = null!;
    public virtual ApplicationUser ReviewerUser { get; set; } = null!;
}