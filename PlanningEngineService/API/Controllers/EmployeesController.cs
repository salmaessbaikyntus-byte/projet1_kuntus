using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanningEngineService.Infrastructure.Data;
using PlanningEngineService.Domain.Entities;

namespace PlanningEngineService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly PlanningDbContext _context;

    public EmployeesController(PlanningDbContext context)
    {
        _context = context;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<IActionResult> GetEmployees()
    {
        var employees = await _context.Employees.ToListAsync();
        return Ok(employees);
    }

    // POST: api/employees
    [HttpPost]
    [HttpPost]
public async Task<IActionResult> CreateEmployee(Employee employee)
{
    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();
    
    // On retourne l'ID créé pour être conforme au standard REST
    return Ok(employee); 
}
}