using AnalyticsService.Application.Interfaces;
using AnalyticsService.Domain;
using AnalyticsService.Engine;

namespace AnalyticsService.Application.UseCases;

/// <summary>
/// Simulation What-if : applique des hypothèses sur les données sans modifier la source.
/// Lecture seule.
/// </summary>
public class RunSimulationUseCase
{
    private readonly IPlanningDataProvider _planningData;
    private readonly KpiCalculator _kpiCalculator;

    public RunSimulationUseCase(IPlanningDataProvider planningData, KpiCalculator kpiCalculator)
    {
        _planningData = planningData;
        _kpiCalculator = kpiCalculator;
    }

    /// <summary>
    /// Simule l'impact si N employés supplémentaires étaient absents.
    /// Ne modifie aucune donnée.
    /// </summary>
    public async Task<SimulationResultDto> SimulateAbsencesAsync(
        DateTime start,
        DateTime end,
        int assumedAbsentCount,
        CancellationToken ct = default)
    {
        var assignments = await _planningData.GetAssignmentsAsync(start, end, null, ct);

        var employeesPresent = assignments.Select(a => a.EmployeeId).Distinct().Count();
        var simulatedPresent = Math.Max(0, employeesPresent - assumedAbsentCount);

        var kpis = _kpiCalculator.Compute(assignments, start, end);

        return new SimulationResultDto
        {
            PeriodStart = start,
            PeriodEnd = end,
            OriginalKpis = kpis,
            SimulationType = "Absences",
            AssumedAbsentCount = assumedAbsentCount,
            OriginalEmployeeCount = employeesPresent,
            SimulatedEmployeeCount = simulatedPresent,
            Note = "Les KPI affichés sont basés sur les données réelles. La simulation d'absences n'affecte pas le calcul des % (effectif réel utilisé)."
        };
    }
}

public class SimulationResultDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public KpiResult OriginalKpis { get; set; } = new();
    public string SimulationType { get; set; } = string.Empty;
    public int AssumedAbsentCount { get; set; }
    public int OriginalEmployeeCount { get; set; }
    public int SimulatedEmployeeCount { get; set; }
    public string Note { get; set; } = string.Empty;
}
