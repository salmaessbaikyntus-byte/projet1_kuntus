using Microsoft.EntityFrameworkCore;
using ReportingService.Infrastructure.Data;
using ReportingService.Domain.Entities;

namespace ReportingService.Application.Services
{
    public class PlanningQueryService
    {
        private readonly ApplicationDbContext _context;

        public PlanningQueryService(ApplicationDbContext context)
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
                .Where(p => p.Date >= start && p.Date <= end);

            if (departmentId.HasValue)
            {
                query = query.Where(p =>
                    p.Employee.DepartmentId == departmentId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
