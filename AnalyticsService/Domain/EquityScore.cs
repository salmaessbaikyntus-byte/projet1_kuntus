namespace AnalyticsService.Domain;

/// <summary>
/// Score d'équité inter-semaine (variance par employé).
/// </summary>
public class EquityScore
{
    public double Score { get; set; }
    public double Variance { get; set; }
    public int EmployeesAnalyzed { get; set; }
    public Dictionary<Guid, int> AssignmentsPerEmployee { get; set; } = new();
}
