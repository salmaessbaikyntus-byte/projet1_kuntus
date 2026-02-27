using ReportingService.Application.Interfaces;
using ReportingService.Domain.Entities;

namespace ReportingService.Application.UseCases;

/// <summary>
/// Récupère l'historique versionné des rapports.
/// </summary>
public class GetReportHistoryUseCase
{
    private readonly IReportRepository _repository;

    public GetReportHistoryUseCase(IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReportHistoryDto> ExecuteAsync(DateTime? periodStart = null, DateTime? periodEnd = null, CancellationToken ct = default)
    {
        IEnumerable<Report> reports;

        if (periodStart.HasValue && periodEnd.HasValue)
            reports = await _repository.GetByPeriodAsync(periodStart.Value, periodEnd.Value);
        else
            reports = await _repository.GetAllAsync();

        return new ReportHistoryDto
        {
            Reports = reports.Select(r => new ReportSummaryDto
            {
                Id = r.Id,
                Type = r.Type,
                Category = r.Category,
                PeriodStart = r.PeriodStart,
                PeriodEnd = r.PeriodEnd,
                Status = r.Status.ToString(),
                VersionHash = r.VersionHash,
                CreatedAt = r.CreatedAt,
                ObsoleteAt = r.ObsoleteAt,
                GeneratedBy = r.GeneratedBy
            }).ToList()
        };
    }
}

public class ReportHistoryDto
{
    public List<ReportSummaryDto> Reports { get; set; } = new();
}

public class ReportSummaryDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string Status { get; set; } = string.Empty;
    public string VersionHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ObsoleteAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
}
