using System.Text.Json.Serialization;

namespace PlanningEngineService.Domain.Entities;

/// <summary>
/// Affectation d'une pause (PSS) à un employé pour une date donnée.
/// StartTime/EndTime dérivés de AssignmentDate + PssCodeValue.
/// </summary>
public class PlanningAssignment
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }

    /// <summary>Date de la pause (jour ouvré).</summary>
    public DateOnly AssignmentDate { get; set; }

    /// <summary>Valeur de l'enum PssCode (0-15).</summary>
    public int PssCodeValue { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    [JsonIgnore]
    public Employee? Employee { get; set; }
}