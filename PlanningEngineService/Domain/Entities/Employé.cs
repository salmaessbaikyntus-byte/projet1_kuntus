namespace PlanningEngineService.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;

    public Guid ServiceUnitId { get; set; }
}
