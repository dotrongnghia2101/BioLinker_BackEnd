namespace BioLinker.DTO.PaymentDTO
{
    public class PayOSRequest
    {
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
        public string? PlanId { get; set; }
        public string? ItemName { get; set; }
    }
}
