namespace BioLinker.Enities
{
    public class Theme
    {
        public string? ThemeId { get; set; } = Guid.NewGuid().ToString();// PK
        public string PrimaryColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
    }
}
