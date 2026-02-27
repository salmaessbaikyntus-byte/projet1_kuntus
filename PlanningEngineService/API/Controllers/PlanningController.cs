using Microsoft.AspNetCore.Mvc;
using PlanningEngineService.Application.DTOs;
using PlanningEngineService.Application.UseCases;

namespace PlanningEngineService.Controllers;

/// <summary>
/// API métier officielle de génération de planning.
/// Aucune logique métier : orchestration uniquement.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PlanningController : ControllerBase
{
    private readonly GenerateWeeklyPlanningUseCase _generateWeekUseCase;

    public PlanningController(GenerateWeeklyPlanningUseCase generateWeekUseCase)
    {
        _generateWeekUseCase = generateWeekUseCase;
    }

    /// <summary>
    /// Génère le planning hebdomadaire de pauses pour une semaine donnée.
    /// Retourne le planning par jour, les statistiques (% pauses, PSS sensibles).
    /// </summary>
    /// <param name="request">Requête avec date de début de semaine et employés présents par jour.</param>
    /// <param name="ct">Token d'annulation.</param>
    [HttpPost("generate-week")]
    [ProducesResponseType(typeof(GenerateWeeklyPlanningResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GenerateWeeklyPlanningResponse>> GenerateWeek(
        [FromBody] GenerateWeeklyPlanningRequest request,
        CancellationToken ct)
    {
        try
        {
            var result = await _generateWeekUseCase.ExecuteAsync(request, ct);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
