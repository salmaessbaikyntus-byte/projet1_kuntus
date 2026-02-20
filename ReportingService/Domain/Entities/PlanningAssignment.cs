using System;

namespace ReportingService.Domain.Entities
{
    public class PlanningAssignment
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan LunchStart { get; set; }
        public TimeSpan LunchEnd { get; set; }

        public string SpecialCaseType { get; set; }
    }
}
