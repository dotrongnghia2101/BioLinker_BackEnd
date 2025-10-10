using BioLinker.Helper;

namespace BioLinker.DTO.TemplateDTO
{
    public class CreateTemplateDetail
    {
        public string TemplateId { get; set; } = default!;
        public string? ElementType { get; set; }
        public Position? Position { get; set; }
        public Size? Size { get; set; }
        public StyleConfig? Style { get; set; }
        public ElementContent? Element { get; set; }
        public int OrderIndex { get; set; }
    }
}
