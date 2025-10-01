namespace BioLinker.Enities
{
    public class StyleSettings
    {
        public string? StyleSettingsId { get; set; } = Guid.NewGuid().ToString();   
        public string? StyleId { get; set; }

        public string? Thumbnail { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public bool? CookieBanner { get; set; }

        // Navigation
        public virtual Style? Style { get; set; }
    }
}
