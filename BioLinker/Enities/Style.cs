namespace BioLinker.Enities
{
    public class Style
    {
        public string? StyleId { get; set; } = Guid.NewGuid().ToString();// PK
        public string Preset { get; set; } = string.Empty;
        public string? LayoutMode { get; set; } // flex-vertical, flex-horizontal, absolute
        public string? ButtonColor { get; set; }
        public string? IconColor { get; set; }
        public string? BackgroundColor { get; set; }

        // Navigation
        public virtual BioPage? BioPage { get; set; }
    }
}
