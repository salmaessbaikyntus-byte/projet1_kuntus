[HttpGet]
public async Task<IActionResult> GetPlanning(
    Guid serviceUnitId,
    DateTime start,
    DateTime end)
{
    var planning = await _context.PlanningAssignments
        .Include(p => p.Employee)
        .ThenInclude(e => e.ServiceUnit)
        .Where(p =>
            p.Employee.ServiceUnitId == serviceUnitId &&
            p.Date >= start &&
            p.Date <= end)
        .Select(p => new PlanningViewDto
        {
            EmployeeName = p.Employee.FullName,
            Date = p.Date,
            Lunch = $"{p.LunchStart:hh\\:mm} - {p.LunchEnd:hh\\:mm}",
            SpecialCaseType = p.SpecialCaseType,
            ServiceName = p.Employee.ServiceUnit.Name
        })
        .OrderBy(p => p.Date)
        .ThenBy(p => p.EmployeeName)
        .ToListAsync();

    return Ok(planning);
}
