namespace BioLinker.DTO.UserDTO
{
    public class UserTemplateCreate
    {
        public string? UserId { get; set; }
        public string? TemplateId { get; set; }
        public decimal PricePaid { get; set; }
        public string? Currency { get; set; }
        public int? DurationInDays { get; set; }
    }
}
