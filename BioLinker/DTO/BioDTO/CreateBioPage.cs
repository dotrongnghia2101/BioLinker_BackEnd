namespace BioLinker.DTO.BioDTO
{
    public class CreateBioPage
    {
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }

        public BackgroundCreate? Background { get; set; }
        public CreateStyle? Style { get; set; }
        public CreateStyleSettings? StyleSettings { get; set; }
        public List<CreateContent>? Contents { get; set; }
    }
}
