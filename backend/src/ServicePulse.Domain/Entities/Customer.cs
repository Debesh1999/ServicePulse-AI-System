using System.ComponentModel.DataAnnotations;

namespace ServicePulse.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }

    [MaxLength(80)]
    public string ExternalReference { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(120)]
    public string ContactMasked { get; set; } = string.Empty;

    // Navigation
    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();
}