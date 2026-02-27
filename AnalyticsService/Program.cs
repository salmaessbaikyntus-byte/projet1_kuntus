using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using AnalyticsService.Application.Interfaces;
using AnalyticsService.Application.UseCases;
using AnalyticsService.Engine;
using AnalyticsService.Infrastructure;
using AnalyticsService.Infrastructure.Data;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<PlanningEngineOptions>(
    builder.Configuration.GetSection(PlanningEngineOptions.SectionName));

builder.Services.AddHttpClient<IPlanningDataProvider, PlanningEngineApiDataProvider>(client =>
{
    var baseUrl = builder.Configuration[$"{PlanningEngineOptions.SectionName}:BaseUrl"] ?? "http://localhost:5088";
    client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<KpiCalculator>();
builder.Services.AddScoped<EquityCalculator>();
builder.Services.AddScoped<ComplianceChecker>();
builder.Services.AddScoped<AnomalyDetector>();
builder.Services.AddScoped<ComputeKpisUseCase>();
builder.Services.AddScoped<RunSimulationUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();
