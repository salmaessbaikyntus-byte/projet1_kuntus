namespace AnalyticsService.Domain;

/// <summary>
/// Résultat du respect de la règle des 10 %.
/// </summary>
public class ComplianceResult
{
    public bool IsCompliant { get; set; }
    public double MaxObservedPercentage { get; set; }
    public int ViolationsCount { get; set; }
    public int TotalDaysAnalyzed { get; set; }
    public int CompliantDaysCount { get; set; }
}
