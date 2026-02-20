using Microsoft.AspNetCore.Mvc;
using AnalyticsService.Application.Services;

namespace AnalyticsService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly PlanningQueryService _planningQuery;
        private readonly TenPercentEngine _tenPercent;
        private readonly FairnessEngine _fairness;

        public AnalyticsController(
            PlanningQueryService planningQuery,
            TenPercentEngine tenPercent,
            FairnessEngine fairness)
        {
            _planningQuery = planningQuery;
            _tenPercent = tenPercent;
            _fairness = fairness;
        }

        [HttpGet("kpi")]
        public async Task<IActionResult> GetKpis(
            [FromQuery] DateTime start, 
            [FromQuery] DateTime end, 
            [FromQuery] Guid? departmentId = null)
        {
            try 
            {
                var planning = await _planningQuery.GetPlanningAsync(start, end, departmentId);

                if (planning == null || !planning.Any())
                    return NotFound("Aucune donnée de planning trouvée pour cette période.");

                // Calculs via les moteurs de règles
                _tenPercent.Evaluate(planning);
                var fairnessScore = _fairness.CalculateScore(planning);

                return Ok(new
                {
                    PeriodStart = start,
                    PeriodEnd = end,
                    TenPercentRule = new
                    {
                        IsValid = _tenPercent.IsValid,
                        MaxObservedPercentage = _tenPercent.MaxObservedPercentage,
                        Violations = _tenPercent.ViolationsCount // Utile pour l'interface
                    },
                    FairnessScore = fairnessScore,
                    TotalAssignments = planning.Count,
                    GeneratedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }
    }
}