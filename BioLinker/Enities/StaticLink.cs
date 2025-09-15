namespace BioLinker.Enities
{
    public class StaticLink
    {
        public string? StaticLinkId { get; set; } = Guid.NewGuid().ToString();// PK
        public string? Title { get; set; } = string.Empty;
        public string? Icon { get; set; } = string.Empty;
        public string? Platform { get; set; } = string.Empty;
        public string? DefaultUrl { get; set; } = string.Empty;
    }
}
