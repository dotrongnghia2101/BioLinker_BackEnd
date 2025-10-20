namespace BioLinker.DTO.UserDTO
{
    public class UserTemplateResponse
    {
        public string UTemplateId { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? TemplateId { get; set; }
        public decimal PricePaid { get; set; }
        public string? Currency { get; set; }
        public DateTime PurchaseAt { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
