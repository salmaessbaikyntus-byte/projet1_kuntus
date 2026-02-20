public class Employee
{
    public Guid Id { get; set; }

    public string Matricule { get; set; }
    public string FullName { get; set; }

    public Guid DepartmentId { get; set; }
    public Guid FloorId { get; set; }
    public Guid ServiceUnitId { get; set; }

    public string ContractType { get; set; }
    public bool IsActive { get; set; }
}
