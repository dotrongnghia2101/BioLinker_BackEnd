namespace BioLinker.Enities
{
    public class StyleColor
    {
        public string? StyleColorId { get; set; } = Guid.NewGuid().ToString();
        public string? StyleId { get; set; }

        public string? ButtonColor { get; set; }
        public string? IconColor { get; set; }
        public string? BackgroundColor { get; set; }

        // Navigation
        public virtual Style? Style { get; set; }
    }
}
