namespace AnalyticsService.Domain;

/// <summary>
/// Détection d'anomalies : sur-utilisation, patterns récurrents.
/// </summary>
public class AnomalyDetectionResult
{
    public List<OverusedEmployee> OverusedEmployees { get; set; } = new();
    public List<string> RecurringAnomalies { get; set; } = new();
}

public class OverusedEmployee
{
    public Guid EmployeeId { get; set; }
    public int AssignmentCount { get; set; }
    public double ExpectedAverage { get; set; }
    public string Reason { get; set; } = string.Empty;
}
