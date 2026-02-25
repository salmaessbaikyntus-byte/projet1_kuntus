using System.Text.Json.Serialization;

namespace PlanningEngineService.Domain.Entities; // Vérifie que c'est bien ton namespace

public class PlanningAssignment
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    
    // Le [JsonIgnore] empêche Swagger de te demander l'objet complet
    [JsonIgnore]
    public Employee? Employee { get; set; } 
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}