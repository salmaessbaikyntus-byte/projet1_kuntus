namespace PlanningEngineService.Domain.Entities;

/// <summary>
/// Historique glissant par employé et par semaine.
/// Ne jamais reset sans raison métier explicite.
/// </summary>
public class EmployeeWeeklyStats
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public int WeekNumber { get; set; }
    public int Year { get; set; }
    public int SensitivePssCount { get; set; }

    public Employee? Employee { get; set; }
}
