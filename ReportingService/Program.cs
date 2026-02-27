using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using ReportingService.Application.Interfaces;
using ReportingService.Application.UseCases;
using ReportingService.Infrastructure;
using ReportingService.Infrastructure.Data;
using ReportingService.Infrastructure.Export;
using ReportingService.Infrastructure.Repositories;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<PlanningEngineOptions>(builder.Configuration.GetSection(PlanningEngineOptions.SectionName));
builder.Services.Configure<AnalyticsOptions>(builder.Configuration.GetSection(AnalyticsOptions.SectionName));

builder.Services.AddHttpClient<IPlanningDataProvider, PlanningEngineApiDataProvider>(c =>
{
    var url = builder.Configuration[$"{PlanningEngineOptions.SectionName}:BaseUrl"] ?? "http://localhost:5088";
    c.BaseAddress = new Uri(url.TrimEnd('/') + "/");
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<IAnalyticsClient, AnalyticsHttpClient>(c =>
{
    var url = builder.Configuration[$"{AnalyticsOptions.SectionName}:BaseUrl"] ?? "http://localhost:5090";
    c.BaseAddress = new Uri(url.TrimEnd('/') + "/");
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IPdfReportGenerator, PdfReportGenerator>();
builder.Services.AddScoped<IExcelReportGenerator, ExcelReportGenerator>();
builder.Services.AddScoped<GenerateReportUseCase>();
builder.Services.AddScoped<GetReportHistoryUseCase>();
builder.Services.AddScoped<MarkReportsObsoleteUseCase>();

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