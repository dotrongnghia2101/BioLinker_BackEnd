namespace BioLinker.Enities
{
    public class AnalyticLinkClick
    {
        public string ClickId { get; set; } = Guid.NewGuid().ToString(); // PK
        public string StaticLinkId { get; set; } = default!;             // FK -> StaticLink
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual StaticLink StaticLink { get; set; } = default!;

    }
}
