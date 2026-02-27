using System;

namespace AnalyticsService.Domain.Entities;

/// <summary>
/// Modèle aligné avec PlanningEngineService (source de vérité).
/// PssCodeValue 0 = PSS11, 15 = PSS44 (sensibles).
/// </summary>
public class PlanningAssignment
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime AssignmentDate { get; set; }
    public int PssCodeValue { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    /// <summary>Type d'affectation : Standard, Conges, etc.</summary>
    public string SpecialCaseType { get; set; } = "Standard";

    public Employee? Employee { get; set; }

    /// <summary>PSS11 (0) ou PSS44 (15) = créneaux sensibles.</summary>
    public bool IsSensitive => PssCodeValue == 0 || PssCodeValue == 15;
}
