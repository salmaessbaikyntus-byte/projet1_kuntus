using System.Collections.Generic;

namespace PlanningEngineService.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // ✅ relation inverse
    public ICollection<PlanningAssignment> PlanningAssignments { get; set; }
        = new List<PlanningAssignment>();
}