namespace BioLinker.Enities
{
    public class UserTemplate
    {
        public string? UTemplateId { get; set; } = Guid.NewGuid().ToString();// PK
        public string? UserId { get; set; } // FK
        public string TemplateId { get; set; } // FK
        public DateTime PurchaseAt { get; set; }
        public decimal PricePaid { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime? ExpireDate { get; set; }

        // Navigation
        public virtual User? User { get; set; }
        public virtual Template? Template { get; set; }
    }
}
