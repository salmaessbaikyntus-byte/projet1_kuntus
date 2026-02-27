using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportingService.Application.Interfaces;

namespace ReportingService.Infrastructure;

public class AnalyticsHttpClient : IAnalyticsClient
{
    private readonly HttpClient _http;
    private readonly ILogger<AnalyticsHttpClient> _logger;
    private readonly AnalyticsOptions _options;

    public AnalyticsHttpClient(HttpClient http, IOptions<AnalyticsOptions> options, ILogger<AnalyticsHttpClient> logger)
    {
        _http = http;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<KpiSummaryDto?> GetKpisAsync(DateTime start, DateTime end, CancellationToken ct = default)
    {
        var url = $"{_options.BaseUrl.TrimEnd('/')}/api/analytics/kpi?start={start:yyyy-MM-dd}&end={end:yyyy-MM-dd}";
        try
        {
            var response = await _http.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AnalyticsKpiResponse>(ct);
            if (result?.Kpis == null)
                return null;

            var k = result.Kpis;
            return new KpiSummaryDto
            {
                AverageBreakPercentage = k.AverageBreakPercentage,
                PeakBreakPercentage = k.PeakBreakPercentage,
                CompliantDaysPercentage = k.CompliantDaysPercentage,
                TotalAssignments = k.TotalAssignments,
                TotalSensitivePssCount = k.TotalSensitivePssCount,
                EquityScore = k.Equity?.Score ?? 0,
                IsCompliant = k.Compliance?.IsCompliant ?? true
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Impossible de récupérer les KPI depuis Analytics {Url}", url);
            return null;
        }
    }
}

public class AnalyticsOptions
{
    public const string SectionName = "Analytics";
    public string BaseUrl { get; set; } = "http://localhost:5090";
}

public class AnalyticsKpiResponse
{
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public AnalyticsKpiResult? Kpis { get; set; }
}

public class AnalyticsKpiResult
{
    public double AverageBreakPercentage { get; set; }
    public double PeakBreakPercentage { get; set; }
    public double CompliantDaysPercentage { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalSensitivePssCount { get; set; }
    public AnalyticsEquityResult? Equity { get; set; }
    public AnalyticsComplianceResult? Compliance { get; set; }
}

public class AnalyticsEquityResult
{
    public double Score { get; set; }
}

public class AnalyticsComplianceResult
{
    public bool IsCompliant { get; set; }
}
