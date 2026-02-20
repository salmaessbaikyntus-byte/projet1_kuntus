using Microsoft.EntityFrameworkCore;
using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Infrastructure.Data
{
    public class AnalyticsDbContext : DbContext
    {
        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
            : base(options)
        {
        }

        // Tables organisationnelles
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<PlanningAssignment> PlanningAssignments { get; set; }

        // Table KPI snapshots
        public DbSet<KpiSnapshot> KpiSnapshots { get; set; }
    }
}
