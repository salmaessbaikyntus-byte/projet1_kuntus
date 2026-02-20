using Microsoft.EntityFrameworkCore;
using ReportingService.Infrastructure.Data;
using ReportingService.Application.Interfaces;
using ReportingService.Infrastructure.Repositories;
using ReportingService.Application.Services;
using ReportingService.Infrastructure.Export;

// 1. Indispensable pour éviter les erreurs de format de date avec PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// 2. Configuration du CORS (Autorise votre page HTML à appeler l'API)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Configuration de la base de données
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. Injection de dépendances (Enregistrement unique de chaque service)
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<PdfExportService>();
builder.Services.AddScoped<ReportGenerationService>();

var app = builder.Build();

// 5. Pipeline de configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Affiche les erreurs détaillées
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Activation du CORS
app.UseCors();

app.UseStaticFiles(); // Pour servir vos fichiers HTML/CSS si placés dans wwwroot
app.UseAuthorization();
app.MapControllers();

app.Run();