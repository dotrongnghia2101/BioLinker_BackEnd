namespace BioLinker.DTO.PaymentDTO
{
    public class PayOSRequest
    {
        public long OrderCode { get; set; } 
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string ItemName { get; set; }
    }
}
