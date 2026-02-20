using ReportingService.Application.Enums;

namespace ReportingService.Application.DTOs
{
    public class GenerateReportRequest
    {
        public ReportCategory Category { get; set; }
        public ReportType Type { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public string GeneratedFor { get; set; }
        public string GeneratedBy { get; set; }
    }
}
