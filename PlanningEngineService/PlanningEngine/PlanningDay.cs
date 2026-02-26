namespace PlanningEngineService.PlanningEngine;

public class PlanningDay
{
    public int DayIndex { get; init; } // 1..5 (lun–ven)

    public Dictionary<PssCode, PssSlot> Slots { get; init; } = new();
}