using AnalyticsService.Infrastructure.Data;
using AnalyticsService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsService.Application.Services
{
    public class PlanningQueryService
    {
        private readonly AnalyticsDbContext _context;

        public PlanningQueryService(AnalyticsDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlanningAssignment>> GetPlanningAsync(
            DateTime start,
            DateTime end,
            Guid? departmentId = null)
        {
            var query = _context.PlanningAssignments
                .Include(p => p.Employee)
                .Where(p => p.AssignmentDate >= start && p.AssignmentDate <= end);

            if (departmentId.HasValue)
            {
                query = query.Where(p =>
                    p.Employee.DepartmentId == departmentId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
