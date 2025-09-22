namespace BioLinker.Enities
{
    public class Wallpaper
    {
        public string? WallpaperId { get; set; } = Guid.NewGuid().ToString();// PK
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        // Navigation
        public virtual ICollection<BioPage> BioPages { get; set; } = new List<BioPage>();
    }
}
