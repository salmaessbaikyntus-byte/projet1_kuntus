namespace PlanningEngineService.Application.DTOs;

public class PssAssignmentDto
{
    public Guid EmployeeId { get; set; }
    public int DayIndex { get; set; }
    public string PssCode { get; set; } = string.Empty;
    public DateOnly AssignmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
