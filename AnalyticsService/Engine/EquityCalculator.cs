using AnalyticsService.Domain;
using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Engine;

/// <summary>
/// Calcule le score d'équité (variance des affectations par employé).
/// </summary>
public class EquityCalculator
{
    public EquityScore Calculate(List<PlanningAssignment> assignments)
    {
        var perEmployee = assignments
            .GroupBy(a => a.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Count());

        if (perEmployee.Count == 0)
        {
            return new EquityScore
            {
                Score = 100,
                Variance = 0,
                EmployeesAnalyzed = 0
            };
        }

        var counts = perEmployee.Values.ToList();
        var mean = counts.Average();
        var variance = counts.Average(c => Math.Pow(c - mean, 2));
        var score = Math.Max(0, Math.Min(100, 100 - variance * 10));

        return new EquityScore
        {
            Score = Math.Round(score, 2),
            Variance = Math.Round(variance, 4),
            EmployeesAnalyzed = perEmployee.Count,
            AssignmentsPerEmployee = perEmployee
        };
    }
}
