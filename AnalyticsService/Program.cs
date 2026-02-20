using Microsoft.EntityFrameworkCore;
using AnalyticsService.Infrastructure.Data;
using AnalyticsService.Application.Services; // Ajustez selon votre dossier réel

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<KpiService>();
builder.Services.AddScoped<PlanningQueryService>();
builder.Services.AddScoped<TenPercentEngine>();
builder.Services.AddScoped<FairnessEngine>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();
