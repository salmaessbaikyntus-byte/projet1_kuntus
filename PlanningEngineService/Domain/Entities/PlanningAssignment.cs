namespace PlanningEngineService.Domain.Entities;

public class PlanningAssignment
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }

    public DateTime Date { get; set; }
    public TimeSpan LunchStart { get; set; }
    public TimeSpan LunchEnd { get; set; }

    public string SpecialCaseType { get; set; } = "Standard";
}
