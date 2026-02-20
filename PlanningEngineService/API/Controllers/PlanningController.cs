using Microsoft.EntityFrameworkCore; // Assure-toi que cet import est présent
using PlanningEngineService.Application.DTOs;

[HttpGet]
public async Task<IActionResult> GetPlanning([FromQuery] string semaine, [FromQuery] string? jour)
{
    // On récupère les données brutes avec les relations
    var query = _context.PlanningAssignments.AsQueryable();

    // Filtrage par semaine (puisque ton entité utilise 'Semaine' en string)
    if (!string.IsNullOrEmpty(semaine))
    {
        query = query.Where(p => p.Semaine == semaine);
    }

    if (!string.IsNullOrEmpty(jour))
    {
        query = query.Where(p => p.Jour == jour);
    }

    var result = await query.Select(p => new PlanningViewDto
    {
        // On adapte selon tes entités réelles
        EmployeeName = p.EmployeeId, // Ou p.Employee.FullName si tu as la relation
        Date = DateTime.Now, // On garde une date par défaut ou on parse la semaine
        Lunch = p.Dejeuner ?? "Non défini",
        SpecialCaseType = p.TypeShift,
        ServiceName = "Service RH"
    })
    .ToListAsync();

    return Ok(result);
}