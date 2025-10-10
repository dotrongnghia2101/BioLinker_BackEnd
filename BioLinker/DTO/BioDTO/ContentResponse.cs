using BioLinker.Helper;

namespace BioLinker.DTO.BioDTO
{
    public class ContentResponse
    {
        public string? ContentId { get; set; }
        public string? BioPageId { get; set; }
        public string? ElementType { get; set; }
        public string? Alignment { get; set; }
        public bool Visible { get; set; }

        public Position? Position { get; set; }
        public Size? Size { get; set; }
        public StyleConfig? Style { get; set; }
        public ElementContent? Element { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
