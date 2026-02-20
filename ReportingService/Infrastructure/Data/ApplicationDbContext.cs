using Microsoft.EntityFrameworkCore;
using ReportingService.Domain.Entities;

namespace ReportingService.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Reporting
        public DbSet<Report> Reports { get; set; }

        // Organisation / Planning
        public DbSet<Employee> Employees { get; set; }
        public DbSet<PlanningAssignment> PlanningAssignments { get; set; }
    }
}
