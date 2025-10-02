namespace BioLinker.Enities
{
    public class Background
    {
        public string? BackgroundId { get; set; } = Guid.NewGuid().ToString();
        public string? BioPageId { get; set; }

        public string? Type { get; set; }  // image | color | gradient
        public string? Value { get; set; } // URL hoặc mã màu
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<BioPage> BioPages { get; set; } =new List<BioPage>();
    }
}
