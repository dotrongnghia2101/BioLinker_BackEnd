namespace BioLinker.Enities
{
    public class Payment
    {
        public string? PaymentId { get; set; } = Guid.NewGuid().ToString();// PK
        public string? UserId { get; set; } // FK
        public string? PlanId { get; set; } // FK
        public decimal? Amount { get; set; }
        public string? Method { get; set; } = string.Empty;
        public string OrderCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PaymentUrl { get; set; }
        public string? Checksum { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? TransactionId { get; set; }
        public string? Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }


        // Navigation
        public virtual User? User { get; set; }
        public virtual SubscriptionPlan? Plan { get; set; }
    }
}
