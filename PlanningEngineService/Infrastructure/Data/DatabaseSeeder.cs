using Microsoft.EntityFrameworkCore;
using PlanningEngineService.Domain.Entities;

namespace PlanningEngineService.Infrastructure.Data;

/// <summary>
/// Seed réaliste pour DEV/TEST/DEMO - Divisions → Départements → Services → Employés.
/// Adapté au schéma actuel (Employees, ServiceUnits).
/// Idempotent : ne seed que si les tables sont vides.
/// </summary>
public static class DatabaseSeeder
{
    // GUIDs fixes pour éviter les doublons à chaque run
    private static readonly Guid[] ServiceUnitIds =
    {
        new("a0000001-0000-0000-0000-000000000001"), // Backend
        new("a0000001-0000-0000-0000-000000000002"), // Frontend
        new("a0000001-0000-0000-0000-000000000003"), // Mobile
        new("a0000001-0000-0000-0000-000000000004"), // Réseau
        new("a0000001-0000-0000-0000-000000000005"), // Cloud & DevOps
        new("a0000001-0000-0000-0000-000000000006"), // BI
        new("a0000001-0000-0000-0000-000000000007"), // ML
        new("a0000001-0000-0000-0000-000000000008"), // Paie
        new("a0000001-0000-0000-0000-000000000009"), // Admin RH
        new("a0000001-0000-0000-0000-00000000000a"), // Recrutement
        new("a0000001-0000-0000-0000-00000000000b"), // Comptabilité
        new("a0000001-0000-0000-0000-00000000000c"), // Contrôle
        new("a0000001-0000-0000-0000-00000000000d"), // Helpdesk
        new("a0000001-0000-0000-0000-00000000000e"), // Logistique
        new("a0000001-0000-0000-0000-00000000000f"), // Digital Marketing
        new("a0000001-0000-0000-0000-000000000010"), // Communication
    };

    private static readonly (Guid Id, string Name)[] Employees =
    {
        (new Guid("b0000001-0000-0000-0000-000000000001"), "Salma Sbai"),
        (new Guid("b0000001-0000-0000-0000-000000000002"), "Yassine Bennani"),
        (new Guid("b0000001-0000-0000-0000-000000000003"), "Imane El Fassi"),
        (new Guid("b0000001-0000-0000-0000-000000000004"), "Karim Amrani"),
        (new Guid("b0000001-0000-0000-0000-000000000005"), "Omar Zerouali"),
        (new Guid("b0000001-0000-0000-0000-000000000006"), "Nadia Chakir"),
        (new Guid("b0000001-0000-0000-0000-000000000007"), "Hassan Toumi"),
        (new Guid("b0000001-0000-0000-0000-000000000008"), "Amina Rahmani"),
        (new Guid("b0000001-0000-0000-0000-000000000009"), "Sofia El Idrissi"),
        (new Guid("b0000001-0000-0000-0000-00000000000a"), "Mohamed El Kadi"),
        (new Guid("b0000001-0000-0000-0000-00000000000b"), "Rachid Bouzidi"),
        (new Guid("b0000001-0000-0000-0000-00000000000c"), "Samir Lahlou"),
        (new Guid("b0000001-0000-0000-0000-00000000000d"), "Khadija Benali"),
        (new Guid("b0000001-0000-0000-0000-00000000000e"), "Youssef Majid"),
        (new Guid("b0000001-0000-0000-0000-00000000000f"), "Leila Saidi"),
    };

    private static readonly string[] ServiceUnitNames =
    {
        "Backend", "Frontend", "Mobile", "Réseau", "Cloud & DevOps",
        "Business Intelligence", "Machine Learning", "Paie", "Administration RH", "Recrutement",
        "Comptabilité Générale", "Contrôle de gestion", "Helpdesk", "Logistique",
        "Digital Marketing", "Communication"
    };

    public static async Task SeedAsync(PlanningDbContext context)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // ServiceUnits
            if (!await context.ServiceUnits.AnyAsync())
            {
                for (var i = 0; i < ServiceUnitIds.Length; i++)
                {
                    context.ServiceUnits.Add(new ServiceUnit
                    {
                        Id = ServiceUnitIds[i],
                        Name = ServiceUnitNames[i]
                    });
                }
                await context.SaveChangesAsync();
            }

            // Employees (schéma actuel: Id, Name uniquement)
            if (!await context.Employees.AnyAsync())
            {
                foreach (var (id, name) in Employees)
                {
                    context.Employees.Add(new Employee
                    {
                        Id = id,
                        Name = name
                    });
                }
                await context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
