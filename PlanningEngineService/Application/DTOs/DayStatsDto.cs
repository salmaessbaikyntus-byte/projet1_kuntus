namespace PlanningEngineService.Application.DTOs;

public class DayStatsDto
{
    public int DayIndex { get; set; }
    public DateOnly Date { get; set; }
    public int PresentEmployees { get; set; }
    public int EmployeesOnBreak { get; set; }
    public double BreakPercentage { get; set; }
    public int SensitivePssCount { get; set; }
}
