using Microsoft.EntityFrameworkCore;
using PlanningEngineService.Application.DTOs;
using PlanningEngineService.Domain.Entities;
using PlanningEngineService.Infrastructure.Data;
using PlanningEngineService.PlanningEngine;

namespace PlanningEngineService.Application.UseCases;

/// <summary>
/// Use Case CQRS : génération du planning hebdomadaire de pauses.
/// Responsabilités :
/// - Récupérer les employés présents par jour
/// - Charger l'historique glissant depuis la DB
/// - Appeler PlanningEngine.GenerateWeek
/// - Sauvegarder PlanningAssignments et EmployeeWeeklyStats
/// </summary>
public class GenerateWeeklyPlanningUseCase
{
    private const int SlidingHistoryWeeks = 4;
    private readonly PlanningDbContext _db;

    public GenerateWeeklyPlanningUseCase(PlanningDbContext db)
    {
        _db = db;
    }

    public async Task<GenerateWeeklyPlanningResponse> ExecuteAsync(GenerateWeeklyPlanningRequest request, CancellationToken ct = default)
    {
        // 1. Validation
        ValidateRequest(request);

        var (year, weekNumber) = GetWeekNumber(request.WeekStartDate);

        // 2. Charger l'historique glissant (N dernières semaines)
        var allEmployeeIds = request.EmployeesByDay.Values
            .SelectMany(x => x)
            .Distinct()
            .ToList();

        var history = await LoadSlidingHistoryAsync(allEmployeeIds, year, weekNumber, ct);

        // Copie pour calculer le delta de PSS sensibles cette semaine
        var historyBeforeEngine = history.ToDictionary(k => k.Key, v => v.Value.SensitivePssCount);

        // 3. Appeler le moteur (logique métier dans PlanningEngine uniquement)
        var engine = new PlanningEngine.PlanningEngine();
        var planningWeek = engine.GenerateWeek(request.EmployeesByDay, history);

        // 4. Supprimer les affectations existantes pour cette semaine
        var weekDates = GetWeekDates(request.WeekStartDate);
        await DeleteExistingAssignmentsForWeekAsync(weekDates, ct);

        // 5. Persister les nouvelles affectations
        var assignments = BuildAssignments(planningWeek, request.WeekStartDate);
        await _db.PlanningAssignments.AddRangeAsync(assignments, ct);

        // 6. Persister l'historique glissant (EmployeeWeeklyStats) — count par semaine
        var sensitiveThisWeek = history.ToDictionary(
            h => h.Key,
            h => h.Value.SensitivePssCount - (historyBeforeEngine.TryGetValue(h.Key, out var before) ? before : 0));
        await UpsertEmployeeWeeklyStatsAsync(sensitiveThisWeek, year, weekNumber, ct);

        await _db.SaveChangesAsync(ct);

        // 7. Construire la réponse DTO
        return BuildResponse(planningWeek, request.WeekStartDate, request.EmployeesByDay);
    }

    private static void ValidateRequest(GenerateWeeklyPlanningRequest request)
    {
        if (request.WeekStartDate.DayOfWeek != DayOfWeek.Monday)
            throw new ArgumentException("WeekStartDate doit être un lundi.", nameof(request));

        for (int d = 1; d <= 5; d++)
        {
            if (!request.EmployeesByDay.ContainsKey(d))
                request.EmployeesByDay[d] = new List<Guid>();
        }
    }

    private async Task<Dictionary<Guid, EmployeeHistory>> LoadSlidingHistoryAsync(
        List<Guid> employeeIds,
        int currentYear,
        int currentWeekNumber,
        CancellationToken ct)
    {
        var stats = await _db.EmployeeWeeklyStats
            .Where(s => employeeIds.Contains(s.EmployeeId))
            .Where(s => (s.Year < currentYear) || (s.Year == currentYear && s.WeekNumber < currentWeekNumber))
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.WeekNumber)
            .Take(employeeIds.Count * SlidingHistoryWeeks)
            .ToListAsync(ct);

        var history = new Dictionary<Guid, EmployeeHistory>();

        foreach (var empId in employeeIds)
        {
            var empStats = stats
                .Where(s => s.EmployeeId == empId)
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.WeekNumber)
                .Take(SlidingHistoryWeeks)
                .ToList();

            var totalSensitive = empStats.Sum(s => s.SensitivePssCount);

            history[empId] = new EmployeeHistory
            {
                EmployeeId = empId,
                SensitivePssCount = totalSensitive
            };
        }

        return history;
    }

    private static (int Year, int WeekNumber) GetWeekNumber(DateOnly date)
    {
        var dt = date.ToDateTime(TimeOnly.MinValue);
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        var rule = culture.DateTimeFormat.CalendarWeekRule;
        var firstDOW = culture.DateTimeFormat.FirstDayOfWeek;
        var weekNumber = calendar.GetWeekOfYear(dt, rule, firstDOW);
        return (date.Year, weekNumber);
    }

    private static IEnumerable<DateOnly> GetWeekDates(DateOnly weekStart)
    {
        for (int i = 0; i < 5; i++)
            yield return weekStart.AddDays(i);
    }

    private async Task DeleteExistingAssignmentsForWeekAsync(IEnumerable<DateOnly> dates, CancellationToken ct)
    {
        var dateList = dates.ToList();
        var toDelete = await _db.PlanningAssignments
            .Where(pa => dateList.Contains(pa.AssignmentDate))
            .ToListAsync(ct);
        _db.PlanningAssignments.RemoveRange(toDelete);
    }

    private static List<PlanningAssignment> BuildAssignments(PlanningWeek planningWeek, DateOnly weekStart)
    {
        var result = new List<PlanningAssignment>();
        var tz = TimeZoneInfo.Local;

        foreach (var day in planningWeek.Days)
        {
            var assignmentDate = weekStart.AddDays(day.DayIndex - 1);

            foreach (var (pssCode, slot) in day.Slots)
            {
                foreach (var employeeId in slot.EmployeesOnPause)
                {
                    var (start, end) = PssTimeMapper.ToUtc(assignmentDate, pssCode, tz);

                    result.Add(new PlanningAssignment
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = employeeId,
                        AssignmentDate = assignmentDate,
                        PssCodeValue = (int)pssCode,
                        StartTime = start,
                        EndTime = end
                    });
                }
            }
        }

        return result;
    }

    private async Task UpsertEmployeeWeeklyStatsAsync(
        Dictionary<Guid, int> sensitiveThisWeek,
        int year,
        int weekNumber,
        CancellationToken ct)
    {
        foreach (var (empId, count) in sensitiveThisWeek.Where(x => x.Value > 0))
        {
            var existing = await _db.EmployeeWeeklyStats
                .FirstOrDefaultAsync(s => s.EmployeeId == empId && s.Year == year && s.WeekNumber == weekNumber, ct);

            if (existing != null)
                existing.SensitivePssCount = count;
            else
            {
                _db.EmployeeWeeklyStats.Add(new EmployeeWeeklyStats
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = empId,
                    Year = year,
                    WeekNumber = weekNumber,
                    SensitivePssCount = count
                });
            }
        }
    }

    private static GenerateWeeklyPlanningResponse BuildResponse(
        PlanningWeek planningWeek,
        DateOnly weekStart,
        Dictionary<int, List<Guid>> employeesByDay)
    {
        var assignments = new List<PssAssignmentDto>();
        var dayStats = new List<DayStatsDto>();
        var totalSensitive = 0;
        var sensitivePss = new HashSet<PssCode> { PssCode.PSS11, PssCode.PSS44 };

        foreach (var day in planningWeek.Days)
        {
            var assignmentDate = weekStart.AddDays(day.DayIndex - 1);
            var presentEmployees = employeesByDay.TryGetValue(day.DayIndex, out var list) ? list.Count : 0;
            var employeesOnBreak = day.Slots.Values.SelectMany(s => s.EmployeesOnPause).Distinct().Count();
            var daySensitiveCount = day.Slots
                .Where(kv => sensitivePss.Contains(kv.Key))
                .Sum(kv => kv.Value.EmployeesOnPause.Count);

            totalSensitive += daySensitiveCount;

            foreach (var (pssCode, slot) in day.Slots)
            {
                foreach (var empId in slot.EmployeesOnPause)
                {
                    var (startTime, endTime) = PssTimeMapper.GetTimeWindow(pssCode);
                    assignments.Add(new PssAssignmentDto
                    {
                        EmployeeId = empId,
                        DayIndex = day.DayIndex,
                        PssCode = pssCode.ToString(),
                        AssignmentDate = assignmentDate,
                        StartTime = startTime,
                        EndTime = endTime
                    });
                }
            }

            dayStats.Add(new DayStatsDto
            {
                DayIndex = day.DayIndex,
                Date = assignmentDate,
                PresentEmployees = presentEmployees,
                EmployeesOnBreak = employeesOnBreak,
                BreakPercentage = presentEmployees > 0 ? (100.0 * employeesOnBreak / presentEmployees) : 0,
                SensitivePssCount = daySensitiveCount
            });
        }

        return new GenerateWeeklyPlanningResponse
        {
            WeekStartDate = weekStart,
            Assignments = assignments,
            DayStats = dayStats,
            TotalSensitivePss = totalSensitive
        };
    }
}
