namespace BioLinker.Enities
{
    public class Style
    {
        public string? StyleId { get; set; } = Guid.NewGuid().ToString();// PK
        public string Preset { get; set; } = string.Empty;
        public string TextStyle { get; set; } = string.Empty;
        public string ButtonStyle { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
}
