using AnalyticsService.Domain;
using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Engine;

/// <summary>
/// Calcule les KPI métier à partir du planning (source PlanningEngine).
/// Ne recalcule jamais le planning.
/// </summary>
public class KpiCalculator
{
    public KpiResult Compute(List<PlanningAssignment> assignments, DateTime periodStart, DateTime periodEnd)
    {
        if (assignments.Count == 0)
        {
            return new KpiResult
            {
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                ComputedAt = DateTime.UtcNow,
                Compliance = new ComplianceResult { TotalDaysAnalyzed = 0 }
            };
        }

        var presentByDate = ComputePresentByDate(assignments);
        var compliance = new ComplianceChecker().Check(assignments, presentByDate);
        var equity = new EquityCalculator().Calculate(assignments);

        var breakPercentages = new List<double>();
        var peakPercentage = 0.0;
        var slots = GenerateTimeSlots(8, 20, 15);

        foreach (var dayGroup in assignments.GroupBy(a => a.AssignmentDate.Date))
        {
            var dayAssignments = dayGroup.ToList();
            var present = presentByDate.GetValueOrDefault(dayGroup.Key, dayAssignments.Select(a => a.EmployeeId).Distinct().Count());
            if (present == 0) continue;

            foreach (var slot in slots)
            {
                var onBreak = dayAssignments.Count(a =>
                    a.StartTime.TimeOfDay <= slot && a.EndTime.TimeOfDay > slot);
                var pct = 100.0 * onBreak / present;
                breakPercentages.Add(pct);
                if (pct > peakPercentage) peakPercentage = pct;
            }
        }

        return new KpiResult
        {
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            AverageBreakPercentage = breakPercentages.Count > 0 ? Math.Round(breakPercentages.Average(), 2) : 0,
            PeakBreakPercentage = Math.Round(peakPercentage, 2),
            CompliantDaysPercentage = compliance.TotalDaysAnalyzed > 0
                ? Math.Round(100.0 * compliance.CompliantDaysCount / compliance.TotalDaysAnalyzed, 2)
                : 100,
            TotalAssignments = assignments.Count,
            TotalSensitivePssCount = assignments.Count(a => a.IsSensitive),
            SensitivePssPercentage = assignments.Count > 0
                ? Math.Round(100.0 * assignments.Count(a => a.IsSensitive) / assignments.Count, 2)
                : 0,
            Equity = equity,
            Compliance = compliance,
            ComputedAt = DateTime.UtcNow
        };
    }

    private static Dictionary<DateTime, int> ComputePresentByDate(List<PlanningAssignment> assignments)
    {
        return assignments
            .GroupBy(a => a.AssignmentDate.Date)
            .ToDictionary(g => g.Key, g => g.Select(a => a.EmployeeId).Distinct().Count());
    }

    private static List<TimeSpan> GenerateTimeSlots(int startHour, int endHour, int stepMinutes)
    {
        var slots = new List<TimeSpan>();
        var current = TimeSpan.FromHours(startHour);
        var end = TimeSpan.FromHours(endHour);
        while (current < end)
        {
            slots.Add(current);
            current = current.Add(TimeSpan.FromMinutes(stepMinutes));
        }
        return slots;
    }
}
