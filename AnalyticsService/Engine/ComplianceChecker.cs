using AnalyticsService.Domain;
using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Engine;

/// <summary>
/// Vérifie le respect de la règle des 10 %.
/// </summary>
public class ComplianceChecker
{
    private const double TenPercentLimit = 10.0;

    public ComplianceResult Check(
        List<PlanningAssignment> assignments,
        Dictionary<DateTime, int>? presentByDate = null)
    {
        if (assignments.Count == 0)
        {
            return new ComplianceResult
            {
                IsCompliant = true,
                MaxObservedPercentage = 0,
                ViolationsCount = 0,
                TotalDaysAnalyzed = 0,
                CompliantDaysCount = 0
            };
        }

        presentByDate ??= assignments
            .GroupBy(a => a.AssignmentDate.Date)
            .ToDictionary(g => g.Key, g => g.Select(a => a.EmployeeId).Distinct().Count());

        var slots = GenerateTimeSlots(8, 20, 15);
        var maxPercentage = 0.0;
        var violationsCount = 0;
        var daysAnalyzed = 0;
        var compliantDays = 0;

        foreach (var dayGroup in assignments.GroupBy(a => a.AssignmentDate.Date))
        {
            var date = dayGroup.Key;
            var dayAssignments = dayGroup.ToList();
            var present = presentByDate.GetValueOrDefault(date, dayAssignments.Select(a => a.EmployeeId).Distinct().Count());
            if (present == 0) continue;

            daysAnalyzed++;
            var dayCompliant = true;

            foreach (var slot in slots)
            {
                var onBreak = dayAssignments.Count(a =>
                    a.StartTime.TimeOfDay <= slot && a.EndTime.TimeOfDay > slot);
                var pct = 100.0 * onBreak / present;
                if (pct > maxPercentage) maxPercentage = pct;
                if (pct > TenPercentLimit)
                {
                    violationsCount++;
                    dayCompliant = false;
                }
            }

            if (dayCompliant) compliantDays++;
        }

        return new ComplianceResult
        {
            IsCompliant = violationsCount == 0,
            MaxObservedPercentage = Math.Round(maxPercentage, 2),
            ViolationsCount = violationsCount,
            TotalDaysAnalyzed = daysAnalyzed,
            CompliantDaysCount = compliantDays
        };
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
