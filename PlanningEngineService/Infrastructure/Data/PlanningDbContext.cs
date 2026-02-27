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
    public DbSet<EmployeeWeeklyStats> EmployeeWeeklyStats => Set<EmployeeWeeklyStats>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<PlanningAssignment>(entity =>
        {
            entity.HasOne(pa => pa.Employee)
                .WithMany(e => e.PlanningAssignments)
                .HasForeignKey(pa => pa.EmployeeId);

            entity.HasIndex(pa => new { pa.AssignmentDate, pa.EmployeeId }).IsUnique();
        });

        modelBuilder.Entity<EmployeeWeeklyStats>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.EmployeeId, e.Year, e.WeekNumber }).IsUnique();
            entity.HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}