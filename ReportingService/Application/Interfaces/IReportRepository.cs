using ReportingService.Domain.Entities;

namespace ReportingService.Application.Interfaces
{
    public interface IReportRepository
    {
        Task AddAsync(Report report);
        Task<List<Report>> GetAllAsync();
        Task<Report?> GetByIdAsync(Guid id);
        Task<IEnumerable<Report>> GetByDateAsync(DateTime start, DateTime end);
    }
}
