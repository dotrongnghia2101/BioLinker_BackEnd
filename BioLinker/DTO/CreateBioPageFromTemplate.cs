namespace BioLinker.DTO
{
    public class CreateBioPageFromTemplate
    {
        public string UserId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }
    }
}
