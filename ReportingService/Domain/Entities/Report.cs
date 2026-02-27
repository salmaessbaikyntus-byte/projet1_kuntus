namespace ReportingService.Domain.Entities;

/// <summary>
/// Rapport RH officiel, versionné et traçable.
/// </summary>
public class Report
{
    public Guid Id { get; set; }
    public Guid PlanningId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }

    public string GeneratedFor { get; set; } = string.Empty;
    public string GeneratedBy { get; set; } = string.Empty;
    public string? Reason { get; set; }

    public ReportStatus Status { get; set; }
    public string VersionHash { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;
    public string ExportFormat { get; set; } = "PDF";

    public DateTime CreatedAt { get; set; }
    public DateTime? ObsoleteAt { get; set; }
}

