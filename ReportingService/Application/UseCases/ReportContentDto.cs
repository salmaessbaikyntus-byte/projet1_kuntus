namespace ReportingService.Application.UseCases;

public class ReportContentDto
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalEmployees { get; set; }
    public double MaxBreakPercentage { get; set; }
    public bool TenPercentRuleRespected { get; set; }
    public int SensitivePssUsage { get; set; }
    public string ManagerialSummary { get; set; } = string.Empty;
}
