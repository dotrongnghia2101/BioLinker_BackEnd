namespace BioLinker.Enities
{
    public class BioPage
    {
        public string? BioPageId { get; set; } = Guid.NewGuid().ToString();// PK
        public string? UserId { get; set; } // FK
        public string? TemplateId { get; set; } // FK
        public string? StyleId { get; set; } // FK
        public string? ThemeId { get; set; } // FK
        public string? WallpaperId { get; set; } // FK
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
        public string? Background { get; set; } = string.Empty;
        public string? CustomerDomain { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public virtual User? User { get; set; }
        public virtual Template? Template { get; set; }
        public virtual Style? Style { get; set; }
        public virtual Theme? Theme { get; set; }
        public virtual Wallpaper? Wallpaper { get; set; }

        public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
        public virtual ICollection<Link> Links { get; set; } = new List<Link>();
        public virtual ICollection<AnalyticLink> AnalyticLinks { get; set; } = new List<AnalyticLink>();

    }
}
