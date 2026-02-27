using AnalyticsService.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.API.Controllers;

/// <summary>
/// API Analytics - KPI, conformité, équité, simulations What-if.
/// Ne génère jamais de planning.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly ComputeKpisUseCase _computeKpis;
    private readonly RunSimulationUseCase _runSimulation;

    public AnalyticsController(ComputeKpisUseCase computeKpis, RunSimulationUseCase runSimulation)
    {
        _computeKpis = computeKpis;
        _runSimulation = runSimulation;
    }

    /// <summary>
    /// Calcule les KPI pour une période.
    /// </summary>
    [HttpGet("kpi")]
    [ProducesResponseType(typeof(KpiResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetKpis(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        [FromQuery] Guid? serviceUnitId = null,
        CancellationToken ct = default)
    {
        var result = await _computeKpis.ExecuteAsync(start, end, serviceUnitId, ct);

        if (result.Kpis.TotalAssignments == 0 && result.Kpis.Compliance.TotalDaysAnalyzed == 0)
            return NotFound(new { message = "Aucune donnée de planning trouvée pour cette période." });

        return Ok(result);
    }

    /// <summary>
    /// Simulation What-if : impact d'absences supposées (lecture seule).
    /// </summary>
    [HttpGet("simulate/absences")]
    [ProducesResponseType(typeof(SimulationResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> SimulateAbsences(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end,
        [FromQuery] int assumedAbsentCount = 5,
        CancellationToken ct = default)
    {
        var result = await _runSimulation.SimulateAbsencesAsync(start, end, assumedAbsentCount, ct);
        return Ok(result);
    }
}
