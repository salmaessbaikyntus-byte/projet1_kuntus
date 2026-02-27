using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportingService.Application.Interfaces;
using ReportingService.Application.UseCases;
using ReportingService.Domain.Entities;

namespace ReportingService.Infrastructure.Export;

public class PdfReportGenerator : IPdfReportGenerator
{
    public Task GenerateAsync(string filePath, ReportContentDto content, Report report, CancellationToken ct = default)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Header().Text("ShiftMaster - Rapport RH Officiel").FontSize(22).Bold();
                page.Header().Text("Document confidentiel - Département RH").FontSize(10).Italic();

                page.Content().Column(column =>
                {
                    column.Spacing(12);

                    column.Item().Text("RÉSUMÉ MANAGÉRIAL").FontSize(14).Bold();
                    column.Item().Text(content.ManagerialSummary);

                    column.Item().Text("PÉRIODE").FontSize(12).Bold();
                    column.Item().Text($"Du {content.PeriodStart:dd/MM/yyyy} au {content.PeriodEnd:dd/MM/yyyy}");

                    column.Item().Text("INDICATEURS CLÉS").FontSize(12).Bold();
                    column.Item().Text($"Effectif total : {content.TotalEmployees}");
                    column.Item().Text($"% max en pause : {content.MaxBreakPercentage:F1} %");
                    column.Item().Text($"Règle des 10 % : {(content.TenPercentRuleRespected ? "Respectée" : "Non respectée")}");
                    column.Item().Text($"Usage PSS sensibles : {content.SensitivePssUsage}");

                    column.Item().PaddingTop(20).Text($"Généré par : {report.GeneratedBy}").FontSize(9);
                    column.Item().Text($"Généré le : {report.CreatedAt:dd/MM/yyyy HH:mm} UTC").FontSize(9);
                    if (!string.IsNullOrEmpty(report.Reason))
                        column.Item().Text($"Motif : {report.Reason}").FontSize(9);
                });

                page.Footer().AlignCenter().Text("ShiftMaster - Document officiel - Ne pas diffuser");
            });
        })
        .GeneratePdf(filePath);

        return Task.CompletedTask;
    }
}
