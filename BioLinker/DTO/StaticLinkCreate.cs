namespace BioLinker.DTO
{
    public class StaticLinkCreate
    {
        public string UserId { get; set; } = default!;
        public string? Title { get; set; } = string.Empty;
        public string? Icon { get; set; } = string.Empty;
        public string? Platform { get; set; } = string.Empty;
        public string? DefaultUrl { get; set; } = string.Empty;
        public string? Status {  get; set; } = string.Empty;
    }
}
