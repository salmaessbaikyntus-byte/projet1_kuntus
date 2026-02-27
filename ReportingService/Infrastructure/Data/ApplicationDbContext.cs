using Microsoft.EntityFrameworkCore;
using ReportingService.Domain;
using ReportingService.Domain.Entities;

namespace ReportingService.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<PlanningAssignment> PlanningAssignments => Set<PlanningAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>(e =>
        {
            e.Property(r => r.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<ReportStatus>(v))
                .HasMaxLength(20);
            e.Property(r => r.Reason).HasMaxLength(500);
            e.Property(r => r.ExportFormat).HasMaxLength(10);
        });
    }
}
