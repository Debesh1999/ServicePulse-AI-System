using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePulse.Domain.Entities;

public class Agent
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    [Required]
    [MaxLength(30)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(80)]
    public string TeamName { get; set; } = string.Empty;

    // Navigation
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();
}