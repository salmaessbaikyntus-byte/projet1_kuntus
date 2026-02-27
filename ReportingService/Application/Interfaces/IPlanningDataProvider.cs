namespace ReportingService.Application.Interfaces;

public interface IPlanningDataProvider
{
    Task<IReadOnlyList<PlanningAssignmentDto>> GetAssignmentsAsync(DateTime start, DateTime end, Guid? serviceUnitId, CancellationToken ct = default);
}

public class PlanningAssignmentDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime AssignmentDate { get; set; }
    public int PssCodeValue { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
