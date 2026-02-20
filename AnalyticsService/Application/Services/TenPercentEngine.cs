using AnalyticsService.Domain.Entities;

namespace AnalyticsService.Application.Services
{
    public class TenPercentEngine
    {
        public double MaxObservedPercentage { get; private set; }
        public bool IsValid { get; private set; }

        // 🔥 NOUVELLES PROPRIÉTÉS
        public int ViolationsCount { get; private set; }
        public List<TimeSpan> ViolationSlots { get; private set; } = new();

        public void Evaluate(List<PlanningAssignment> assignments)
        {
            ViolationsCount = 0;
            ViolationSlots.Clear();

            var slots = GenerateSlots(
                TimeSpan.FromHours(11),
                TimeSpan.FromHours(16),
                TimeSpan.FromMinutes(15));

            int totalEmployees = assignments
                .Select(a => a.EmployeeId)
                .Distinct()
                .Count();

            double max = 0;

            foreach (var slot in slots)
            {
                int onBreak = assignments.Count(a =>
                    a.LunchStart <= slot &&
                    a.LunchEnd > slot);

                double percentage = totalEmployees == 0
                    ? 0
                    : (double)onBreak / totalEmployees * 100;

                if (percentage > max)
                    max = percentage;

                if (percentage > 10)
                {
                    ViolationsCount++;
                    ViolationSlots.Add(slot);
                }
            }

            MaxObservedPercentage = max;
            IsValid = ViolationsCount == 0;
        }

        private List<TimeSpan> GenerateSlots(
            TimeSpan start,
            TimeSpan end,
            TimeSpan step)
        {
            var slots = new List<TimeSpan>();
            var current = start;

            while (current < end)
            {
                slots.Add(current);
                current = current.Add(step);
            }

            return slots;
        }
    }
}
