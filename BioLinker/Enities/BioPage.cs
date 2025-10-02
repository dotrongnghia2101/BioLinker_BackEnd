namespace BioLinker.Enities
{
    public class BioPage
    {
        public string? BioPageId { get; set; } = Guid.NewGuid().ToString();// PK
        public string? UserId { get; set; } // FK
        public string? StyleId { get; set; } // FK
        public string? TemplateId { get; set; } // FK
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
        public string? BackgroundId { get; set; } = string.Empty; //FK
        public string? CustomerDomain { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public virtual User? User { get; set; }
        public virtual Template? Template { get; set; }
        public virtual Style? Style { get; set; }   
        public virtual Background? Background { get; set; }
        public virtual StyleSettings? StyleSettings { get; set; }
        public virtual ICollection<Link> Links { get; set; } = new List<Link>();
        public virtual ICollection<AnalyticLink> AnalyticLinks { get; set; } = new List<AnalyticLink>();
        public virtual ICollection<Content> Contents { get; set; } = new List<Content>();

    }
}
