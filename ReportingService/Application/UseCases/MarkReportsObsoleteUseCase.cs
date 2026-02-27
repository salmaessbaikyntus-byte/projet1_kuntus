using ReportingService.Application.Interfaces;

namespace ReportingService.Application.UseCases;

/// <summary>
/// Marque les rapports comme obsolètes lorsque le planning change.
/// </summary>
public class MarkReportsObsoleteUseCase
{
    private readonly IReportRepository _repository;

    public MarkReportsObsoleteUseCase(IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(string planningVersionHash, CancellationToken ct = default)
    {
        await _repository.MarkObsoleteByVersionHashAsync(planningVersionHash, DateTime.UtcNow);
    }
}
