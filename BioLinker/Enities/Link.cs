namespace BioLinker.Enities
{
    public class Link
    {
        public string? LinkId { get; set; } = Guid.NewGuid().ToString(); // PK
        public string? BioPageId { get; set; } // FK
        public string? StaticLinkId { get; set; } // FK (nếu link mặc định)
        public string? Title { get; set; } = string.Empty;
        public string? Url { get; set; } = string.Empty;
        public string? Icon { get; set; } = string.Empty;
        public int? Position { get; set; }
        public int? ClickCount { get; set; }
        public string? Platform { get; set; } = string.Empty;
        public string? LinkType { get; set; } = string.Empty;

        // Navigation
        public virtual BioPage? BioPage { get; set; }
        public virtual ICollection<AnalyticLink> AnalyticLinks { get; set; } = new List<AnalyticLink>();
        public virtual StaticLink? StaticLink { get; set; }
    }
}
