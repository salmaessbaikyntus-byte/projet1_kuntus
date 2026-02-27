using System.Net.Http.Json;
using System.Text.Json.Serialization;
using AnalyticsService.Application.Interfaces;
using AnalyticsService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnalyticsService.Infrastructure;

/// <summary>
/// Récupère le planning depuis l'API PlanningEngineService.
/// </summary>
public class PlanningEngineApiDataProvider : IPlanningDataProvider
{
    private readonly HttpClient _http;
    private readonly ILogger<PlanningEngineApiDataProvider> _logger;
    private readonly PlanningEngineOptions _options;

    public PlanningEngineApiDataProvider(
        HttpClient http,
        IOptions<PlanningEngineOptions> options,
        ILogger<PlanningEngineApiDataProvider> logger)
    {
        _http = http;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<List<PlanningAssignment>> GetAssignmentsAsync(DateTime start, DateTime end, Guid? serviceUnitId = null, CancellationToken ct = default)
    {
        var url = $"api/planningassignments?start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}";
        if (serviceUnitId.HasValue)
            url += $"&serviceUnitId={serviceUnitId}";

        try
        {
            var response = await _http.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            var dtos = await response.Content.ReadFromJsonAsync<List<PlanningAssignmentDto>>(ct) ?? new List<PlanningAssignmentDto>();
            return dtos.Select(Map).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible de récupérer le planning depuis {Url}, retour liste vide", url);
            return new List<PlanningAssignment>();
        }
    }

    private static PlanningAssignment Map(PlanningAssignmentDto dto)
    {
        return new PlanningAssignment
        {
            Id = dto.Id,
            EmployeeId = dto.EmployeeId,
            AssignmentDate = dto.AssignmentDate,
            PssCodeValue = dto.PssCodeValue,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };
    }
}

public class PlanningEngineOptions
{
    public const string SectionName = "PlanningEngine";
    public string BaseUrl { get; set; } = "http://localhost:5088";
}

public class PlanningAssignmentDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("employeeId")]
    public Guid EmployeeId { get; set; }
    [JsonPropertyName("assignmentDate")]
    public DateTime AssignmentDate { get; set; }
    [JsonPropertyName("pssCodeValue")]
    public int PssCodeValue { get; set; }
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }
    [JsonPropertyName("endTime")]
    public DateTime EndTime { get; set; }
}
