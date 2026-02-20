public class PlanningConstraint
{
    public Guid Id { get; set; }

    public Guid DepartmentId { get; set; }
    public Guid ServiceUnitId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int MinAvailabilityPercentage { get; set; } // ex: 90

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
}
