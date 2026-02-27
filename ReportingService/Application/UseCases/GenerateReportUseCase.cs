using ReportingService.Application.DTOs;
using ReportingService.Application.Enums;
using ReportingService.Application.Interfaces;
using ReportingService.Domain;
using ReportingService.Domain.Entities;
using ReportingService.Infrastructure.Export;

namespace ReportingService.Application.UseCases;

/// <summary>
/// Génère un rapport RH officiel (PDF ou Excel).
/// Consomme les données du planning et les KPI Analytics.
/// </summary>
public class GenerateReportUseCase
{
    private readonly IReportRepository _repository;
    private readonly IAnalyticsClient _analyticsClient;
    private readonly IPdfReportGenerator _pdfGenerator;
    private readonly IExcelReportGenerator _excelGenerator;
    private readonly IPlanningDataProvider _planningData;

    public GenerateReportUseCase(
        IReportRepository repository,
        IAnalyticsClient analyticsClient,
        IPdfReportGenerator pdfGenerator,
        IExcelReportGenerator excelGenerator,
        IPlanningDataProvider planningData)
    {
        _repository = repository;
        _analyticsClient = analyticsClient;
        _pdfGenerator = pdfGenerator;
        _excelGenerator = excelGenerator;
        _planningData = planningData;
    }

    public async Task<ReportResultDto> ExecuteAsync(GenerateReportRequest request, CancellationToken ct = default)
    {
        var versionHash = Guid.NewGuid().ToString("N");
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
            VersionHash = versionHash,
            ExportFormat = request.ExportFormat ?? "PDF",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(report);

        var planningData = await _planningData.GetAssignmentsAsync(request.PeriodStart, request.PeriodEnd, null, ct);
        var kpis = await _analyticsClient.GetKpisAsync(request.PeriodStart, request.PeriodEnd, ct);

        var reportContent = new ReportContentDto
        {
            PeriodStart = request.PeriodStart,
            PeriodEnd = request.PeriodEnd,
            TotalEmployees = planningData.Select(a => a.EmployeeId).Distinct().Count(),
            MaxBreakPercentage = kpis?.PeakBreakPercentage ?? 0,
            TenPercentRuleRespected = kpis?.IsCompliant ?? true,
            SensitivePssUsage = kpis?.TotalSensitivePssCount ?? 0,
            ManagerialSummary = BuildManagerialSummary(request, kpis, planningData.Count)
        };

        var storageDir = Path.Combine(AppContext.BaseDirectory, "ReportFiles");
        Directory.CreateDirectory(storageDir);
        var fileName = $"{report.Id}.{(request.ExportFormat == "Excel" ? "xlsx" : "pdf")}";
        var filePath = Path.Combine(storageDir, fileName);

        if (request.ExportFormat == "Excel")
        {
            await _excelGenerator.GenerateAsync(filePath, reportContent, report, ct);
        }
        else
        {
            await _pdfGenerator.GenerateAsync(filePath, reportContent, report, ct);
        }

        report.FilePath = filePath;
        report.Status = ReportStatus.Generated;
        await _repository.UpdateAsync(report);

        return new ReportResultDto
        {
            ReportId = report.Id,
            FilePath = filePath,
            Status = report.Status.ToString(),
            CreatedAt = report.CreatedAt
        };
    }

    private static string BuildManagerialSummary(GenerateReportRequest request, KpiSummaryDto? kpis, int assignmentCount)
    {
        var lines = new List<string>
        {
            $"Rapport {request.Type} - Période du {request.PeriodStart:dd/MM/yyyy} au {request.PeriodEnd:dd/MM/yyyy}.",
            $"Effectif couvert : {assignmentCount} affectations.",
            kpis != null
                ? $"Règle des 10 % : {(kpis.IsCompliant ? "Respectée" : "Non respectée")}. Pic observé : {kpis.PeakBreakPercentage:F1} %."
                : "Indicateurs KPI non disponibles.",
            kpis != null ? $"Score d'équité : {kpis.EquityScore:F1}/100. PSS sensibles : {kpis.TotalSensitivePssCount}." : ""
        };
        return string.Join(" ", lines.Where(s => !string.IsNullOrEmpty(s)));
    }
}

public class ReportResultDto
{
    public Guid ReportId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
