public class Shift
{
    public Guid Id { get; set; }

    public string Code { get; set; } // PSS11
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public string ShiftType { get; set; } // Morning / Afternoon
}
