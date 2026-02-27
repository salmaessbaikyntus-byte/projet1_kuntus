namespace AnalyticsService.Domain;

/// <summary>
/// Résultat agrégé des KPI pour une période.
/// </summary>
public class KpiResult
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public double AverageBreakPercentage { get; set; }
    public double PeakBreakPercentage { get; set; }
    public double CompliantDaysPercentage { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalSensitivePssCount { get; set; }
    public double SensitivePssPercentage { get; set; }
    public EquityScore Equity { get; set; } = new();
    public ComplianceResult Compliance { get; set; } = new();
    public DateTime ComputedAt { get; set; }
}
