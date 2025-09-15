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
        public string? BackGround { get; set; } = string.Empty;
        public string? CustomerDomain { get; set; } = string.Empty;
        public string? Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
