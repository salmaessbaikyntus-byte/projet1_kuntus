using Microsoft.EntityFrameworkCore;
using ReportingService.Application.Interfaces;
using ReportingService.Domain.Entities;
using ReportingService.Infrastructure.Data;

namespace ReportingService.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Report>> GetAllAsync()
        {
            return await _context.Reports.ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(Guid id)
        {
            return await _context.Reports.FindAsync(id);
        }

        public async Task UpdateAsync(Report report)
        {
            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Report>> GetByDateAsync(DateTime start, DateTime end)
        {
            return await _context.Reports
                .Where(r => r.CreatedAt >= start && r.CreatedAt <= end)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetByPeriodAsync(DateTime periodStart, DateTime periodEnd)
        {
            return await _context.Reports
                .Where(r => r.PeriodStart <= periodEnd && r.PeriodEnd >= periodStart)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkObsoleteByVersionHashAsync(string versionHash, DateTime obsoleteAt)
        {
            var reports = await _context.Reports
                .Where(r => r.VersionHash == versionHash && r.Status != Domain.ReportStatus.Obsolete)
                .ToListAsync();
            foreach (var r in reports)
            {
                r.Status = Domain.ReportStatus.Obsolete;
                r.ObsoleteAt = obsoleteAt;
            }
            await _context.SaveChangesAsync();
        }
    }
}
