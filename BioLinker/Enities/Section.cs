namespace BioLinker.Enities
{
    public class Section
    {
        public string? SectionId { get; set; } = Guid.NewGuid().ToString(); // PK
        public string? BioPageId { get; set; } // FK
        public string? Type { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        public string? SubTitle { get; set; } = string.Empty;
        public int? Position { get; set; }
        public bool? Visible { get; set; }

        // Navigation
        public virtual BioPage? BioPage { get; set; }

    }
}
