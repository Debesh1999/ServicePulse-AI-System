using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServicePulse.Domain.Enums;

namespace ServicePulse.Domain.Entities;

public class AnalysisResult
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(60)]
    public string AnalyzerVersion { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Sentiment { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string RiskLevel { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    [Column(TypeName = "decimal(5,2)")]
    public decimal QualityScore { get; set; }

    [MaxLength(2000)]
    public string? Summary { get; set; }

    public string? DetectedKeywordsJson { get; set; }

    [Required]
    [MaxLength(20)]
    public string AnalysisStatus { get; set; } = "Succeeded";

    [MaxLength(1500)]
    public string? ErrorMessage { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Explicit FK for EF Core
    public Guid CallId { get; set; }

    // Navigation
    public virtual Call Call { get; set; } = null!;
}