namespace BioLinker.Enities
{
    public class SubscriptionPlan
    {
        public string? PlanId { get; set; } = Guid.NewGuid().ToString();
        public string? PlanName { get; set; }
        public double? Price { get; set; }
        public int? Duration { get; set; }
        public DurationUnit DurationUnit { get; set; }


        // Navigation
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public enum DurationUnit
    {
        Month,
        Year
    }

    
}
