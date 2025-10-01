namespace BioLinker.Enities
{
    public class StyleText
    {
        public string? StyleTextId { get; set; } = Guid.NewGuid().ToString();
        public string? StyleId { get; set; }

        public string? StyleType { get; set; } // Title | Heading | Paragraph | Button
        public string? CssClass { get; set; }

        // Navigation
        public virtual Style? Style { get; set; }
    }
}
