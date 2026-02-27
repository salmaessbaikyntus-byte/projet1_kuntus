namespace PlanningEngineService.Application.DTOs;

/// <summary>
/// Requête de génération du planning hebdomadaire.
/// </summary>
public class GenerateWeeklyPlanningRequest
{
    /// <summary>Date du lundi de la semaine (ISO 8601).</summary>
    public DateOnly WeekStartDate { get; set; }

    /// <summary>
    /// Employés présents par jour (1=lun, 2=mar, 3=mer, 4=jeu, 5=ven).
    /// Clé = DayIndex (1-5), Valeur = liste des EmployeeId.
    /// </summary>
    public Dictionary<int, List<Guid>> EmployeesByDay { get; set; } = new();
}
