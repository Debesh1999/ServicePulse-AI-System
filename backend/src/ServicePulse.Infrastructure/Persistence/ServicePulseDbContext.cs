using Microsoft.EntityFrameworkCore;
using ServicePulse.Domain.Entities;

namespace ServicePulse.Infrastructure.Persistence;

public class ServicePulseDbContext : DbContext
{
    public ServicePulseDbContext(DbContextOptions<ServicePulseDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Call> Calls { get; set; }
    public DbSet<CallTranscript> CallTranscripts { get; set; }
    public DbSet<AnalysisResult> AnalysisResults { get; set; }
    public DbSet<ManagerReview> ManagerReviews { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("ApplicationUsers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(e => e.DisplayName).HasMaxLength(120).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(30).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure Agent
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.ToTable("Agents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeCode).HasMaxLength(30).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(120).IsRequired();
            entity.Property(e => e.TeamName).HasMaxLength(80);
            entity.HasIndex(e => e.EmployeeCode).IsUnique();
            entity.HasOne(e => e.User).WithOne().HasForeignKey<Agent>(e => e.UserId);
        });

        // Configure Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalReference).HasMaxLength(80);
            entity.Property(e => e.DisplayName).HasMaxLength(120).IsRequired();
            entity.Property(e => e.ContactMasked).HasMaxLength(120);
            entity.HasIndex(e => e.ExternalReference).IsUnique();
        });

        // Configure Call
        modelBuilder.Entity<Call>(entity =>
        {
            entity.ToTable("Calls");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalCallId).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(30).IsRequired();
            entity.Property(e => e.DurationSeconds).HasDefaultValue(0);
            entity.Property(e => e.ResolutionStatus).HasMaxLength(30);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartedAtUtc);
            entity.HasOne(e => e.Agent).WithMany(a => a.Calls).HasForeignKey(e => e.AgentId);
            entity.HasOne(e => e.Customer).WithMany(c => c.Calls).HasForeignKey(e => e.CustomerId);
        });

        // Configure CallTranscript
        modelBuilder.Entity<CallTranscript>(entity =>
        {
            entity.ToTable("CallTranscripts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Text).IsRequired();
            entity.Property(e => e.Source).HasMaxLength(30);
            entity.Property(e => e.LanguageCode).HasMaxLength(10).HasDefaultValue("en");
            entity.Property(e => e.WordCount).HasDefaultValue(0);
            entity.HasIndex(e => e.CallId);
            entity.HasOne(e => e.Call).WithMany(c => c.Transcripts).HasForeignKey(e => e.CallId);
        });

        // Configure AnalysisResult
        modelBuilder.Entity<AnalysisResult>(entity =>
        {
            entity.ToTable("AnalysisResults");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AnalyzerVersion).HasMaxLength(60).IsRequired();
            entity.Property(e => e.Sentiment).HasMaxLength(20).IsRequired();
            entity.Property(e => e.RiskLevel).HasMaxLength(20).IsRequired();
            entity.Property(e => e.QualityScore).HasColumnType("decimal(5,2)");
            entity.Property(e => e.AnalysisStatus).HasMaxLength(20).IsRequired();
            entity.Property(e => e.ErrorMessage).HasMaxLength(1500);
            entity.Property(e => e.Provider).HasMaxLength(30).IsRequired().HasDefaultValue("RuleBased");
            entity.Property(e => e.FallbackReason).HasMaxLength(1500);
            entity.HasIndex(e => e.CallId);
            entity.HasOne(e => e.Call).WithMany(c => c.AnalysisResults).HasForeignKey(e => e.CallId);
        });

        // Configure ManagerReview
        modelBuilder.Entity<ManagerReview>(entity =>
        {
            entity.ToTable("ManagerReviews");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReviewStatus).HasMaxLength(20).IsRequired();
            entity.Property(e => e.CoachingNotes).HasMaxLength(2000);
            entity.Property(e => e.OverrideReason).HasMaxLength(500);
            entity.Property(e => e.ManagerScore).HasColumnType("decimal(5,2)");
            entity.HasIndex(e => e.CallId);
            entity.HasOne(e => e.Call).WithMany(c => c.ManagerReviews).HasForeignKey(e => e.CallId);
            entity.HasOne(e => e.ReviewerUser).WithMany(u => u.ManagerReviews).HasForeignKey(e => e.ReviewerUserId);
        });

        // Configure AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityName).HasMaxLength(80).IsRequired();
            entity.Property(e => e.Action).HasMaxLength(80).IsRequired();
            entity.HasIndex(e => new { e.EntityName, e.EntityId });
        });
    }
}