using PlanningEngineService.PlanningEngine;

public static class PlanningEngineTest
{
    public static void Run()
    {
        var employees = Enumerable.Range(1, 100)
            .Select(_ => Guid.NewGuid())
            .ToList();

        var employeesByDay = new Dictionary<int, List<Guid>>
        {
            { 1, employees },
            { 2, employees },
            { 3, employees },
            { 4, employees },
            { 5, employees }
        };

        var history = new Dictionary<Guid, EmployeeHistory>();

        var engine = new PlanningEngine();

        // 🔁 Simulation de 4 semaines glissantes
        for (int week = 1; week <= 4; week++)
        {
            Console.WriteLine($"====== SEMAINE {week} ======");

            var planningWeek = engine.GenerateWeek(employeesByDay, history);

            int totalSensitive = history.Values.Sum(h => h.SensitivePssCount);

            Console.WriteLine($"Total PSS sensibles cumulés : {totalSensitive}");
            Console.WriteLine();
        }
    }
}