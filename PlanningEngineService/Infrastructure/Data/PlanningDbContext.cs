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
}