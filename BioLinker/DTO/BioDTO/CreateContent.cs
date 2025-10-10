using BioLinker.Helper;

namespace BioLinker.DTO.BioDTO
{
    public class CreateContent
    {
        public string? BioPageId { get; set; }
        public string? ElementType { get; set; }
        public string? Alignment { get; set; }
        public bool Visible { get; set; } = true;

        public Position? Position { get; set; }
        public Size? Size { get; set; }
        public StyleConfig? Style { get; set; }
        public ElementContent? Element { get; set; }
    }
}
