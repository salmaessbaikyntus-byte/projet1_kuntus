public class PlanningEntry
{
    public string Week { get; set; }
    public string Day { get; set; }

    public string EmployeeId { get; set; }

    public string Type { get; set; } // Standard / Cas Tôt / Cas Tard

    public TimeSpan Cafe1Start { get; set; }
    public TimeSpan Cafe1End { get; set; }

    public TimeSpan LunchStart { get; set; }
    public TimeSpan LunchEnd { get; set; }

    public TimeSpan Cafe2Start { get; set; }
    public TimeSpan Cafe2End { get; set; }
}
