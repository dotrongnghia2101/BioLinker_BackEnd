namespace BioLinker.DTO.TemplateDTO
{
    public interface MarketplaceCreate
    {
        public string? TemplateId { get; set; }
        public string? SellerId { get; set; }
        public decimal Price { get; set; }
    }
}
