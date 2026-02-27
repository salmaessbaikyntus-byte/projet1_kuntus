using ReportingService.Domain.Entities;

namespace ReportingService.Application.Interfaces;

public interface IReportRepository
{
    Task AddAsync(Report report);
    Task UpdateAsync(Report report);
    Task<List<Report>> GetAllAsync();
    Task<Report?> GetByIdAsync(Guid id);
    Task<IEnumerable<Report>> GetByDateAsync(DateTime start, DateTime end);
    Task<IEnumerable<Report>> GetByPeriodAsync(DateTime periodStart, DateTime periodEnd);
    Task MarkObsoleteByVersionHashAsync(string versionHash, DateTime obsoleteAt);
}
