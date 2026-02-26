namespace PlanningEngineService.PlanningEngine;

public class PssSlot
{
    public PssCode Code { get; init; }

    public int MaxPauses { get; init; }

    public List<Guid> EmployeesOnPause { get; } = new();

    public bool IsFull => EmployeesOnPause.Count >= MaxPauses;
}