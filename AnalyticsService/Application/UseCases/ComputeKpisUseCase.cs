using AnalyticsService.Application.Interfaces;
using AnalyticsService.Domain;
using AnalyticsService.Engine;

namespace AnalyticsService.Application.UseCases;

/// <summary>
/// Calcule les KPI pour une période donnée.
/// </summary>
public class ComputeKpisUseCase
{
    private readonly IPlanningDataProvider _planningData;
    private readonly KpiCalculator _kpiCalculator;
    private readonly AnomalyDetector _anomalyDetector;

    public ComputeKpisUseCase(
        IPlanningDataProvider planningData,
        KpiCalculator kpiCalculator,
        AnomalyDetector anomalyDetector)
    {
        _planningData = planningData;
        _kpiCalculator = kpiCalculator;
        _anomalyDetector = anomalyDetector;
    }

    public async Task<KpiResultDto> ExecuteAsync(DateTime start, DateTime end, Guid? serviceUnitId = null, CancellationToken ct = default)
    {
        var assignments = await _planningData.GetAssignmentsAsync(start, end, serviceUnitId, ct);

        if (assignments.Count == 0)
        {
            return new KpiResultDto
            {
                PeriodStart = start,
                PeriodEnd = end,
                Kpis = new KpiResult
                {
                    PeriodStart = start,
                    PeriodEnd = end,
                    ComputedAt = DateTime.UtcNow,
                    Compliance = new ComplianceResult { TotalDaysAnalyzed = 0 }
                },
                Anomalies = new Domain.AnomalyDetectionResult()
            };
        }

        var kpis = _kpiCalculator.Compute(assignments, start, end);
        var anomalies = _anomalyDetector.Detect(assignments);

        return new KpiResultDto
        {
            PeriodStart = start,
            PeriodEnd = end,
            Kpis = kpis,
            Anomalies = anomalies
        };
    }
}

public class KpiResultDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public KpiResult Kpis { get; set; } = new();
    public Domain.AnomalyDetectionResult Anomalies { get; set; } = new();
}
