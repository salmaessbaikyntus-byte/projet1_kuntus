using OfficeOpenXml;
using ReportingService.Application.Interfaces;
using ReportingService.Application.UseCases;
using ReportingService.Domain.Entities;

namespace ReportingService.Infrastructure.Export;

public class ExcelReportGenerator : IExcelReportGenerator
{
    public Task GenerateAsync(string filePath, ReportContentDto content, Report report, CancellationToken ct = default)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Rapport RH");

        int row = 1;
        sheet.Cells[row++, 1].Value = "ShiftMaster - Rapport RH";
        sheet.Cells[row, 1].Style.Font.Bold = true;
        row += 2;

        sheet.Cells[row++, 1].Value = "Résumé managérial";
        sheet.Cells[row++, 1].Value = content.ManagerialSummary;
        row++;

        sheet.Cells[row++, 1].Value = "Période";
        sheet.Cells[row++, 1].Value = $"Du {content.PeriodStart:dd/MM/yyyy} au {content.PeriodEnd:dd/MM/yyyy}";
        row++;

        sheet.Cells[row++, 1].Value = "Effectif total";
        sheet.Cells[row, 1].Value = content.TotalEmployees;
        row++;

        sheet.Cells[row++, 1].Value = "% max employés en pause";
        sheet.Cells[row, 1].Value = $"{content.MaxBreakPercentage:F1} %";
        row++;

        sheet.Cells[row++, 1].Value = "Règle des 10 %";
        sheet.Cells[row, 1].Value = content.TenPercentRuleRespected ? "Respectée" : "Non respectée";
        row++;

        sheet.Cells[row++, 1].Value = "Usage PSS sensibles";
        sheet.Cells[row, 1].Value = content.SensitivePssUsage;
        row += 2;

        sheet.Cells[row++, 1].Value = $"Généré par : {report.GeneratedBy}";
        sheet.Cells[row++, 1].Value = $"Généré le : {report.CreatedAt:dd/MM/yyyy HH:mm} UTC";

        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

        package.SaveAs(new FileInfo(filePath));
        return Task.CompletedTask;
    }
}
