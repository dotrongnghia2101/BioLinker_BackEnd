namespace BioLinker.DTO
{
    public class UpdateTemplate
    {
        public string? TemplateId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool IsPremium { get; set; }
        public string? Status { get; set; }
    }
}
