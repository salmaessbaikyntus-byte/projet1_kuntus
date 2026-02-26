namespace PlanningEngineService.PlanningEngine;

public class PlanningEngine
{
    // PSS très tôt / très tard
    private static readonly HashSet<PssCode> SensitivePss =
        new() { PssCode.PSS11, PssCode.PSS44 };

    /// <summary>
    /// Génère un planning hebdomadaire glissant (lun–ven)
    /// </summary>
    public PlanningWeek GenerateWeek(
        Dictionary<int, List<Guid>> employeesByDay,           // day 1..5
        Dictionary<Guid, EmployeeHistory> history              // historique glissant
    )
    {
        var week = new PlanningWeek();

        // 🔹 Initialiser l'historique pour tous les employés
        foreach (var employees in employeesByDay.Values)
        {
            foreach (var empId in employees)
            {
                if (!history.ContainsKey(empId))
                {
                    history[empId] = new EmployeeHistory
                    {
                        EmployeeId = empId,
                        SensitivePssCount = 0
                    };
                }
            }
        }

        // 🔹 Génération jour par jour
        foreach (var (dayIndex, employees) in employeesByDay)
        {
            var day = CreateDay(dayIndex, employees.Count);

            foreach (var employeeId in employees)
            {
                var bestSlot = ChooseBestSlot(
                    day,
                    employeeId,
                    history[employeeId]
                );

                // Affectation de la pause
                bestSlot.EmployeesOnPause.Add(employeeId);
                // Si ce PSS est totalement vide, on évite de l'utiliser tant que possible
            
                // Mise à jour de l'historique glissant
                if (SensitivePss.Contains(bestSlot.Code))
                {
                    history[employeeId].SensitivePssCount++;
                }
            }

            week.Days.Add(day);
        }

        return week;
    }

    /// <summary>
    /// Crée une journée avec la capacité max = 10% des présents
    /// </summary>
    private PlanningDay CreateDay(int dayIndex, int presentEmployees)
    {
        int maxPauses = (int)Math.Floor(presentEmployees * 0.10);

        var day = new PlanningDay
        {
            DayIndex = dayIndex
        };

        foreach (var code in Enum.GetValues<PssCode>()) // Note le <PssCode> ici
{
    // On ignore les codes spécifiques
    if (code == PssCode.PSS12 || code == PssCode.PSS43) continue;

    day.Slots[code] = new PssSlot
    {
        Code = code,
        MaxPauses = maxPauses
    };
}
        return day;
    }

    /// <summary>
    /// Choisit le meilleur PSS selon les contraintes métier
    /// </summary>
    private PssSlot ChooseBestSlot(
        PlanningDay day,
        Guid employeeId,
        EmployeeHistory history
    )
    {
        PssSlot? best = null;
        int bestScore = int.MaxValue;

        foreach (var slot in day.Slots.Values)
        {
            // Contrainte dure : capacité
            if (slot.IsFull)
                continue;

            int score = 0;

            // 🔴 PSS sensibles (très tôt / très tard)
            if (SensitivePss.Contains(slot.Code))
            {
                score += 100;

                // pénalité glissante inter-semaine
                score += history.SensitivePssCount * 200;

                // blocage dur : max 2 sur l'historique
                if (history.SensitivePssCount >= 2)
                    continue;
            }

            // 🔹 Équilibrage de charge
            // Objectif principal : MINIMISER les pauses
score += slot.EmployeesOnPause.Count * 20;

            if (score < bestScore)
            {
                bestScore = score;
                best = slot;
            }
        }

        if (best == null)
        {
            throw new InvalidOperationException(
                "Aucun PSS valide disponible — contraintes trop fortes."
            );
        }

        return best;
    }
}