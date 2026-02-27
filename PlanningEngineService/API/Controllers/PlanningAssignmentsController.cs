using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningEngineService.Infrastructure.Data;
using PlanningEngineService.Domain.Entities;

namespace PlanningEngineService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanningAssignmentsController : ControllerBase
{
    private readonly PlanningDbContext _context;

    public PlanningAssignmentsController(PlanningDbContext context)
    {
        _context = context;
    }

    // GET: api/planningassignments
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? start, [FromQuery] DateTime? end)
    {
        var query = _context.PlanningAssignments.AsQueryable();

        if (start.HasValue)
        {
            var startDate = DateOnly.FromDateTime(start.Value);
            query = query.Where(p => p.AssignmentDate >= startDate);
        }
        if (end.HasValue)
        {
            var endDate = DateOnly.FromDateTime(end.Value);
            query = query.Where(p => p.AssignmentDate <= endDate);
        }

        var assignments = await query
            .Include(p => p.Employee)
            .ToListAsync();

        return Ok(assignments);
    }

    // POST: api/planningassignments
    [HttpPost]
    public async Task<IActionResult> Create(PlanningAssignment assignment)
    {
        // Vérifier que l'employé existe
        var employeeExists = await _context.Employees
            .AnyAsync(e => e.Id == assignment.EmployeeId);

        if (!employeeExists)
            return BadRequest("Employee does not exist");

        _context.PlanningAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        return Ok(assignment);
    }
}