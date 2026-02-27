namespace PlanningEngineService.Application.DTOs;

/// <summary>
/// Réponse de la génération du planning hebdomadaire.
/// </summary>
public class GenerateWeeklyPlanningResponse
{
    public DateOnly WeekStartDate { get; set; }
    public List<PssAssignmentDto> Assignments { get; set; } = new();
    public List<DayStatsDto> DayStats { get; set; } = new();
    public int TotalSensitivePss { get; set; }
}
