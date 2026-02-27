using ReportingService.Application.UseCases;
using ReportingService.Domain.Entities;

namespace ReportingService.Application.Interfaces;

public interface IPdfReportGenerator
{
    Task GenerateAsync(string filePath, ReportContentDto content, Report report, CancellationToken ct = default);
}
