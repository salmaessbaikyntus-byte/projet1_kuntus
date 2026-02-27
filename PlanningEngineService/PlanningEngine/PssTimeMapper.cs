namespace PlanningEngineService.PlanningEngine;

/// <summary>
/// Mapping PSS → plage horaire (08h00–20h00).
/// 16 PSS, 12h = 720 min → 45 min par PSS.
/// </summary>
public static class PssTimeMapper
{
    private static readonly TimeOnly DayStart = new(8, 0);
    private const int MinutesPerPss = 45;

    public static (TimeOnly Start, TimeOnly End) GetTimeWindow(PssCode code)
    {
        var index = (int)code;
        var startMinutes = index * MinutesPerPss;
        var start = DayStart.AddMinutes(startMinutes);
        var end = start.AddMinutes(MinutesPerPss);
        return (start, end);
    }

    public static (DateTime Start, DateTime End) ToUtc(DateOnly date, PssCode code, TimeZoneInfo? tz = null)
    {
        tz ??= TimeZoneInfo.Utc;
        var (startTime, endTime) = GetTimeWindow(code);
        var startDateTime = date.ToDateTime(startTime);
        var endDateTime = date.ToDateTime(endTime);
        var start = TimeZoneInfo.ConvertTimeToUtc(startDateTime, tz);
        var end = TimeZoneInfo.ConvertTimeToUtc(endDateTime, tz);
        return (start, end);
    }
}
