using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServicePulse.Domain.Enums;

namespace ServicePulse.Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Role { get; set; } = string.Empty; // Admin, Manager, Agent

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public byte[]? RowVersion { get; set; }

    // Navigation
    public virtual Agent? Agent { get; set; }
    public virtual ICollection<ManagerReview> ManagerReviews { get; set; } = new List<ManagerReview>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}