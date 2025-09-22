namespace BioLinker.Enities
{
    public class Template
    {
        public string? TemplateId { get; set; } = Guid.NewGuid().ToString();// PK
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsPremium { get; set; }
        public string? CreatedBy { get; set; } // FK -> User
        public string Status { get; set; } = string.Empty;

        // Navigation
        public virtual User? Creator { get; set; }
        public virtual ICollection<UserTemplate> UserTemplates { get; set; } = new List<UserTemplate>();
        public virtual ICollection<BioPage> BioPages { get; set; } = new List<BioPage>();
        public virtual ICollection<Marketplace> Marketplaces { get; set; } = new List<Marketplace>();
    }
}
