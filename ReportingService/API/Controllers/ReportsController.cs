using Microsoft.AspNetCore.Mvc;
using ReportingService.Application.DTOs;
using ReportingService.Application.Interfaces;
using ReportingService.Application.UseCases;

namespace ReportingService.API.Controllers;

/// <summary>
/// API métier des rapports RH officiels.
/// Aucune logique métier dans le controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly GenerateReportUseCase _generateReport;
    private readonly GetReportHistoryUseCase _getHistory;
    private readonly MarkReportsObsoleteUseCase _markObsolete;
    private readonly IReportRepository _repository;

    public ReportsController(
        GenerateReportUseCase generateReport,
        GetReportHistoryUseCase getHistory,
        MarkReportsObsoleteUseCase markObsolete,
        IReportRepository repository)
    {
        _generateReport = generateReport;
        _getHistory = getHistory;
        _markObsolete = markObsolete;
        _repository = repository;
    }

    /// <summary>
    /// Génère un rapport RH (PDF ou Excel).
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(ReportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReportResultDto>> Generate([FromBody] GenerateReportRequest request, CancellationToken ct = default)
    {
        var result = await _generateReport.ExecuteAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Récupère l'historique des rapports, optionnellement filtré par période.
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(ReportHistoryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReportHistoryDto>> GetHistory(
        [FromQuery] DateTime? periodStart,
        [FromQuery] DateTime? periodEnd,
        CancellationToken ct = default)
    {
        var result = await _getHistory.ExecuteAsync(periodStart, periodEnd, ct);
        return Ok(result);
    }

    /// <summary>
    /// Liste tous les rapports.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reports = await _repository.GetAllAsync();
        return Ok(reports);
    }

    /// <summary>
    /// Télécharge un rapport par ID.
    /// </summary>
    [HttpGet("download/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct = default)
    {
        var report = await _repository.GetByIdAsync(id);
        if (report == null)
            return NotFound();

        if (!System.IO.File.Exists(report.FilePath))
            return NotFound("Le fichier physique n'existe pas sur le serveur.");

        var contentType = report.ExportFormat == "Excel" ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
        var fileBytes = await System.IO.File.ReadAllBytesAsync(report.FilePath, ct);
        var fileName = Path.GetFileName(report.FilePath);
        return File(fileBytes, contentType, fileName);
    }

    /// <summary>
    /// Marque les rapports comme obsolètes lorsque le planning change.
    /// </summary>
    [HttpPost("mark-obsolete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkObsolete([FromBody] MarkObsoleteRequest request, CancellationToken ct = default)
    {
        await _markObsolete.ExecuteAsync(request.VersionHash, ct);
        return NoContent();
    }
}

public class MarkObsoleteRequest
{
    public string VersionHash { get; set; } = string.Empty;
}
