namespace PlanningEngineService.Application.DTOs
{
    public class PlanningViewDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Lunch { get; set; } = string.Empty; // Format "12:00 - 13:00"
        public string SpecialCaseType { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
    }
}