using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Application.Services
{
    public class FairnessEngine
    {
        public double CalculateScore(List<PlanningAssignment> assignments)
        {
            var perEmployee = assignments
                .GroupBy(a => a.EmployeeId)
                .Select(g => g.Count(a => a.SpecialCaseType != "Standard"))
                .ToList();

            if (!perEmployee.Any())
                return 100;

            double average = perEmployee.Average();
            double variance = perEmployee.Average(v =>
                Math.Pow(v - average, 2));

            // Score entre 0 et 100
            double score = 100 - variance;

            return Math.Max(0, Math.Min(100, score));
        }
    }
}
