using BioLinker.Helper;

namespace BioLinker.DTO.TemplateDTO
{
    public class TemplateDetailResponse
    {
        public string? TemplateDetailId { get; set; }
        public string? TemplateId { get; set; }
        public string? ElementType { get; set; }
        public Position? Position { get; set; }
        public Size? Size { get; set; }
        public StyleConfig? Style { get; set; }
        public ElementContent? Element { get; set; }
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
