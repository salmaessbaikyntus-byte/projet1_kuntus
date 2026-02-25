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
    public async Task<IActionResult> GetAll()
    {
        var assignments = await _context.PlanningAssignments
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