public class SpecialCaseEngine
{
    public Dictionary<string, int> Analyze(List<PlanningEntry> entries)
    {
        return entries
            .Where(e => e.Type != "Standard")
            .GroupBy(e => e.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public bool IsValid(Dictionary<string,int> stats)
    {
        return stats.All(s => s.Value <= 2);
    }
}
