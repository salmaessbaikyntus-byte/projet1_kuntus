using Microsoft.EntityFrameworkCore;
using PlanningEngineService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURATION DES SERVICES ---
// Ajoute le support des Controllers (pour ton EmployeesController)
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi(); // Gardé car présent dans ton code initial

// Récupération de la chaîne de connexion
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Enregistrement du DbContext PostgreSQL
builder.Services.AddDbContext<PlanningDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// --- 2. CONFIGURATION DU PIPELINE (L'ordre compte !) ---

// Active Swagger uniquement en mode Développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Génère le document technique
    app.UseSwaggerUI(); // Génère l'interface visuelle sur /swagger
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Lie les routes à tes Controllers (Important pour /api/employees)
app.MapControllers();

// Garde ton endpoint de test météo
var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
app.MapGet("/weatherforecast", () =>
{
    return Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )).ToArray();
})
.WithName("GetWeatherForecast");

PlanningEngineTest.Run();

app.Run();

// Modèle de données météo
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}