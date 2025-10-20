namespace BioLinker.DTO.PaymentDTO
{
    public class PayOSRequest
    {
        public string OrderCode { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string? UserId { get; set; } = string.Empty;
        public string? PlanId { get; set; }
    }
}
