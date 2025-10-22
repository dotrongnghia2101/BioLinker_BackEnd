namespace BioLinker.Enities
{
    public class StaticLink
    {
        public string? StaticLinkId { get; set; } = Guid.NewGuid().ToString();// PK
        public string? Title { get; set; } 
        public string? UserId { get; set; }
        public string? Icon { get; set; } 
        public string? Platform { get; set; } 
        public string? DefaultUrl { get; set; }
        public string? Status { get; set; }

        // Navigation
        public virtual User? User { get; set; }
        public virtual ICollection<AnalyticLink>? AnalyticLinks { get; set; }
    }
}
