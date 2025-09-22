namespace BioLinker.Enities
{
    public class Marketplace
    {
        public string? MarketplaceId { get; set; } = Guid.NewGuid().ToString(); // PK
        public string? TemplateId { get; set; } // FK
        public string? SellerId { get; set; } // FK -> User
        public decimal Price { get; set; }
        public int SalesCount { get; set; }

        // Navigation
        public virtual Template? Template { get; set; }
        public virtual User? Seller { get; set; }
    }
}
