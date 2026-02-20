using ReportingService.Domain.Entities;
using ReportingService.Application.Interfaces;
using ReportingService.Infrastructure.Export;

namespace ReportingService.Application.Services
{
    public class ReportGenerationService
    {
        private readonly IReportRepository _repository;
        private readonly PdfExportService _pdfService;

        public ReportGenerationService(
            IReportRepository repository,
            PdfExportService pdfService)
        {
            _repository = repository;
            _pdfService = pdfService;
        }

        public async Task<Report> GenerateAsync(GenerateReportRequest request)
{
    var report = new Report
    {
        Id = Guid.NewGuid(),
        Category = request.Category.ToString(),
        Type = request.Type.ToString(),
        PeriodStart = request.PeriodStart,
        PeriodEnd = request.PeriodEnd,
        GeneratedFor = request.GeneratedFor,
        GeneratedBy = request.GeneratedBy,
        Status = "Processing",
        CreatedAt = DateTime.UtcNow,
        VersionHash = Guid.NewGuid().ToString()
    };

    _context.Reports.Add(report);
    await _context.SaveChangesAsync();

    // Ici plus tard on branchera le vrai générateur PDF/Excel

    report.Status = "Generated";
    report.FilePath = $"reports/{report.Id}.pdf";

    await _context.SaveChangesAsync();

    return report;
}

    }
}