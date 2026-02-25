using Microsoft.EntityFrameworkCore;

using PlanningEngineService.Domain.Entities;

namespace PlanningEngineService.Infrastructure.Data;

public class PlanningDbContext : DbContext
{
    public PlanningDbContext(DbContextOptions<PlanningDbContext> options)
        : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ServiceUnit> ServiceUnits => Set<ServiceUnit>();
    public DbSet<PlanningAssignment> PlanningAssignments => Set<PlanningAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Optional: Specify a default schema if you aren't using 'public'
        // modelBuilder.HasDefaultSchema("reporting");

        // Define Primary Keys & Relationships here
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            // Example: entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

       modelBuilder.Entity<PlanningAssignment>()
    .HasOne(pa => pa.Employee)
    .WithMany(e => e.PlanningAssignments)
    .HasForeignKey(pa => pa.EmployeeId);
    }
}