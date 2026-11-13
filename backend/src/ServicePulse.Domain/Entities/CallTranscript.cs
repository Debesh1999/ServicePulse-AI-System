using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServicePulse.Domain.Enums;

namespace ServicePulse.Domain.Entities;

public class CallTranscript
{
    public Guid Id { get; set; }

    [Required]
    public Guid CallId { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Source { get; set; } = TranscriptSource.ManualUpload.ToString();

    [MaxLength(10)]
    public string LanguageCode { get; set; } = "en";

    [Range(0, int.MaxValue)]
    public int WordCount { get; set; } = 0;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Call Call { get; set; } = null!;
}