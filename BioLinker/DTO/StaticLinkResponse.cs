namespace BioLinker.DTO
{
    public class StaticLinkResponse
    {
        public string StaticLinkId { get; set; } = default!;
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Icon { get; set; }
        public string? Platform { get; set; }
        public string? DefaultUrl { get; set; }
    }
}
