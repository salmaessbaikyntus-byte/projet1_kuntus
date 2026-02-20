public class TenPercentEngine
{
    public double MaxPercentage { get; private set; }
    public bool IsValid { get; private set; }

    public void Evaluate(List<PlanningEntry> entries)
    {
        var slots = GenerateSlots(11, 16, 15);

        int totalEmployees = entries.Select(e => e.EmployeeId).Distinct().Count();

        double worstPercentage = 0;

        foreach (var slot in slots)
        {
            int count = entries.Count(e =>
                e.LunchStart <= slot &&
                e.LunchEnd > slot);

            double percentage = (double)count / totalEmployees * 100;

            if (percentage > worstPercentage)
                worstPercentage = percentage;
        }

        MaxPercentage = worstPercentage;
        IsValid = worstPercentage <= 10;
    }

    private List<TimeSpan> GenerateSlots(int start, int end, int stepMinutes)
    {
        var list = new List<TimeSpan>();
        var current = TimeSpan.FromHours(start);

        while (current < TimeSpan.FromHours(end))
        {
            list.Add(current);
            current = current.Add(TimeSpan.FromMinutes(stepMinutes));
        }

        return list;
    }
}
