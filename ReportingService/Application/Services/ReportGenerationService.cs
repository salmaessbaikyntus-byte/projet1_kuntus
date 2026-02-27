using ReportingService.Domain.Entities;
using ReportingService.Domain;
using ReportingService.Application.Interfaces;
using ReportingService.Application.DTOs;
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
                PlanningId = request.PlanningId,
                Category = request.Category.ToString(),
                Type = request.Type.ToString(),
                PeriodStart = request.PeriodStart,
                PeriodEnd = request.PeriodEnd,
                GeneratedFor = request.GeneratedFor,
                GeneratedBy = request.GeneratedBy,
                Reason = request.Reason,
                Status = ReportStatus.Draft,
                ExportFormat = request.ExportFormat ?? "PDF",
                CreatedAt = DateTime.UtcNow,
                VersionHash = Guid.NewGuid().ToString()
            };

            await _repository.AddAsync(report);

            // Ici plus tard on branchera le vrai générateur PDF/Excel

            report.Status = ReportStatus.Generated;
            report.FilePath = $"reports/{report.Id}.pdf";
            await _repository.UpdateAsync(report);

            return report;
        }

    }
}