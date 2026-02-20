using AnalyticsService.Domain.Entities;
using AnalyticsService.Infrastructure.Data;

namespace AnalyticsService.Application.Services
{
    public class KpiService
    {
        private readonly AnalyticsDbContext _context;

        public KpiService(AnalyticsDbContext context)
        {
            _context = context;
        }

        public async Task<KpiSnapshot> GenerateAsync()
        {
            var totalEmployees = 100;
            var employeesOnBreak = new Random().Next(5, 15);

            var snapshot = new KpiSnapshot
            {
                Id = Guid.NewGuid(),
                TotalEmployees = totalEmployees,
                EmployeesOnBreak = employeesOnBreak,
                BreakPercentage = (double)employeesOnBreak / totalEmployees * 100,
                FairnessScore = new Random().Next(70, 100),
                CreatedAt = DateTime.UtcNow
            };

            _context.KpiSnapshots.Add(snapshot);
            await _context.SaveChangesAsync();

            return snapshot;
        }
    }
}
