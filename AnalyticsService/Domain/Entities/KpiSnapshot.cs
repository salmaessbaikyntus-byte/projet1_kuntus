using System;

namespace AnalyticsService.Domain.Entities
{
    public class KpiSnapshot
    {
        public Guid Id { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        // KPI principaux
        public int TotalEmployees { get; set; }
        public int EmployeesOnBreak { get; set; }

        public double BreakPercentage { get; set; }   // % pause max observé
        public bool TenPercentRuleValid { get; set; }

        public double FairnessScore { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
