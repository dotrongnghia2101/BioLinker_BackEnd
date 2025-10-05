namespace BioLinker.DTO
{
    public class BioPageResponse
    {
        public string? BioPageId { get; set; }
        public string? Title { get; set; }
        public string? UserId { get; set; }
        public string? TemplateId { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public string? CustomerDomain { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public TemplateDTO? Template { get; set; }
        public BackgroundResponse? Background { get; set; }
        public StyleResponse? Style { get; set; }
        public StyleSettingsResponse? StyleSettings { get; set; }
        public List<ContentResponse> Contents { get; set; } = new();
    }
}
