using AnalyticsService.Domain;
using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Engine;

/// <summary>
/// Détecte les anomalies : sur-utilisation, patterns récurrents.
/// </summary>
public class AnomalyDetector
{
    public AnomalyDetectionResult Detect(List<PlanningAssignment> assignments)
    {
        var result = new AnomalyDetectionResult();
        if (assignments.Count == 0) return result;

        var perEmployee = assignments
            .GroupBy(a => a.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Count());
        var expectedAverage = perEmployee.Values.Average();
        var stdDev = StandardDeviation(perEmployee.Values);

        foreach (var (empId, count) in perEmployee)
        {
            if (stdDev > 0 && count > expectedAverage + 2 * stdDev)
            {
                result.OverusedEmployees.Add(new OverusedEmployee
                {
                    EmployeeId = empId,
                    AssignmentCount = count,
                    ExpectedAverage = Math.Round(expectedAverage, 2),
                    Reason = $"Nombre d'affectations ({count}) > moyenne + 2σ"
                });
            }
        }

        var sensitiveCount = assignments.Count(a => a.IsSensitive);
        if (assignments.Count > 0 && (double)sensitiveCount / assignments.Count > 0.3)
        {
            result.RecurringAnomalies.Add(
                $"Utilisation élevée des PSS sensibles : {100.0 * sensitiveCount / assignments.Count:F1} %");
        }

        return result;
    }

    private static double StandardDeviation(IEnumerable<int> values)
    {
        var list = values.ToList();
        if (list.Count < 2) return 0;
        var mean = list.Average();
        return Math.Sqrt(list.Average(v => Math.Pow(v - mean, 2)));
    }
}
