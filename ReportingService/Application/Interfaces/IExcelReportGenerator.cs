using ReportingService.Application.UseCases;
using ReportingService.Domain.Entities;

namespace ReportingService.Application.Interfaces;

public interface IExcelReportGenerator
{
    Task GenerateAsync(string filePath, ReportContentDto content, Report report, CancellationToken ct = default);
}
