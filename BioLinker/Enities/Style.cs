namespace BioLinker.Enities
{
    public class Style
    {
        public string? StyleId { get; set; } = Guid.NewGuid().ToString();// PK
        public string Preset { get; set; } = string.Empty;
        public string? LayoutMode { get; set; } // flex-vertical, flex-horizontal, absolute

        // Navigation
        public virtual ICollection<BioPage> BioPages { get; set; } = new List<BioPage>();
        public virtual ICollection<StyleText> StyleTexts { get; set; } = new List<StyleText>();
        public virtual ICollection<StyleColor> StyleColors { get; set; } = new List<StyleColor>();
        public virtual ICollection<StyleSettings> StyleSettings { get; set; } = new List<StyleSettings>();
    }
}
