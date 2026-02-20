namespace ReportingService.Application.Enums
{
    public enum ReportType
    {
        // Planning
        DailyPlanning,
        WeeklyPlanning,
        MonthlyPlanning,
        TenPercentRule,
        FairnessReport,
        CallCoverage,

        // Employees
        EmployeeList,
        EmployeeByFloor,

        // Leave
        LeaveList,
        LeaveImpact,

        // Performance
        WeeklyKpi,
        MonthlyKpi
    }
}
