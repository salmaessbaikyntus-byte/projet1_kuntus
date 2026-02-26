namespace PlanningEngineService.PlanningEngine;

public class EmployeeHistory
{
    public Guid EmployeeId { get; init; }

    // cumul sur les N dernières semaines
    public int SensitivePssCount { get; set; }
}