namespace ReportingService.Application.Interfaces;

/// <summary>
/// Client pour consommer les KPI du service Analytics.
/// </summary>
public interface IAnalyticsClient
{
    Task<KpiSummaryDto?> GetKpisAsync(DateTime start, DateTime end, CancellationToken ct = default);
}

public class KpiSummaryDto
{
    public double AverageBreakPercentage { get; set; }
    public double PeakBreakPercentage { get; set; }
    public double CompliantDaysPercentage { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalSensitivePssCount { get; set; }
    public double EquityScore { get; set; }
    public bool IsCompliant { get; set; }
}
