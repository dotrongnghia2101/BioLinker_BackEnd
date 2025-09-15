namespace BioLinker.Enities
{
    public class SubscriptionPlan
    {
        public string? PLanId { get; set; } = Guid.NewGuid().ToString();
        public string? PlanName { get; set; }
        public double? Price { get; set; }
        public int? Duration { get; set; }
        public DurationUnit DurationUnit { get; set; }
    }
    public enum DurationUnit
    {
        Month,
        Year
    }
}
