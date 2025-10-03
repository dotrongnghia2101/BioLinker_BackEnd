using BioLinker.Helper;

namespace BioLinker.DTO
{
    public class UpdateContent
    {
        public string? ElementType { get; set; }
        public string? Alignment { get; set; }
        public bool? Visible { get; set; }

        public Position? Position { get; set; }
        public Size? Size { get; set; }
        public StyleConfig? Style { get; set; }
        public ElementContent? Element { get; set; }
    }
}
