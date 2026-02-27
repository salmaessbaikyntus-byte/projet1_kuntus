using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using ReportingService.Application.Interfaces;

namespace ReportingService.Infrastructure;

public class PlanningEngineApiDataProvider : IPlanningDataProvider
{
    private readonly HttpClient _http;
    private readonly PlanningEngineOptions _options;

    public PlanningEngineApiDataProvider(HttpClient http, IOptions<PlanningEngineOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<PlanningAssignmentDto>> GetAssignmentsAsync(DateTime start, DateTime end, Guid? serviceUnitId, CancellationToken ct = default)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/api/planningassignments?start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}";
        try
        {
            var response = await _http.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<List<PlanningAssignmentJson>>(ct) ?? new List<PlanningAssignmentJson>();
            return list.Select(a => new PlanningAssignmentDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                AssignmentDate = a.AssignmentDate,
                PssCodeValue = a.PssCodeValue,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList();
        }
        catch
        {
            return Array.Empty<PlanningAssignmentDto>();
        }
    }
}

public class PlanningEngineOptions
{
    public const string SectionName = "PlanningEngine";
    public string BaseUrl { get; set; } = "http://localhost:5088";
}

public class PlanningAssignmentJson
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
