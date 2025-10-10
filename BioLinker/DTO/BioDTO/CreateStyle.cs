namespace BioLinker.DTO.BioDTO
{
    public class CreateStyle
    {
        public string Preset { get; set; } = string.Empty;
        public string? LayoutMode { get; set; }
        public string? ButtonColor { get; set; }
        public string? IconColor { get; set; }
        public string? ButtonShape { get; set; }
        public string? BackgroundColor { get; set; }
    }
}
