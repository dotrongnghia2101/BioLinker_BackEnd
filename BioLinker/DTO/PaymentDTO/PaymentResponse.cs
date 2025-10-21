namespace BioLinker.DTO.PaymentDTO
{
    public class PaymentResponse
    {
        public string PaymentId { get; set; }
        public string OrderCode { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
        public string? PlanId { get; set; }
        public string? UserId { get; set; }
        public string? PaymentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
