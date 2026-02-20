public class FairnessEngine
{
    public double CalculateScore(List<PlanningEntry> entries)
    {
        var grouped = entries
            .GroupBy(e => e.EmployeeId)
            .Select(g => g.Count(e => e.Type != "Standard"))
            .ToList();

        double avg = grouped.Average();
        double variance = grouped.Average(v => Math.Pow(v - avg, 2));

        return Math.Max(0, 100 - variance);
    }
}
