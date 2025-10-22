namespace BioLinker.DTO.TemplateDTO
{
    public class CreateTemplate
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool IsPremium { get; set; }
        public string? CreatedBy { get; set; }
        public string? Job { get; set; }
        public string? PreviewImage { get; set; }
    }
}
