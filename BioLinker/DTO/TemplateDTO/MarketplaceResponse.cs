namespace BioLinker.DTO.TemplateDTO
{
    public class MarketplaceResponse
    {
        public string MarketplaceId { get; set; } = string.Empty;
        public string? TemplateId { get; set; }
        public string? TemplateName { get; set; }
        public string? SellerId { get; set; }
        public string? SellerName { get; set; }
        public decimal Price { get; set; }
        public int SalesCount { get; set; }
    }
}
