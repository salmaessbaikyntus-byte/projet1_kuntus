using ReportingService.Application.Enums;

namespace ReportingService.Application.DTOs;

public class GenerateReportRequest
{
    public Guid PlanningId { get; set; }
    public ReportCategory Category { get; set; }
    public ReportType Type { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public string GeneratedFor { get; set; } = string.Empty;
    public string GeneratedBy { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? ExportFormat { get; set; } = "PDF";
}
