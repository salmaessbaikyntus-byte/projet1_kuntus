using System;

namespace ReportingService.Domain.Entities
{
    public class Report
    {
        public Guid Id { get; set; }

        // Ajout indispensable pour que ton service compile sans modification
        public Guid PlanningId { get; set; } 

        // Utilisation de "string.Empty" pour supprimer les 7 avertissements (Warnings)
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        public string GeneratedFor { get; set; } = string.Empty;
        public string GeneratedBy { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string VersionHash { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}