using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Application.Interfaces;

/// <summary>
/// Fournit les données de planning (depuis PlanningEngineService ou base locale).
/// Ne recalcule jamais le planning.
/// </summary>
public interface IPlanningDataProvider
{
    Task<List<PlanningAssignment>> GetAssignmentsAsync(DateTime start, DateTime end, Guid? serviceUnitId = null, CancellationToken ct = default);
}
