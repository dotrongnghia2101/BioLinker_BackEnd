namespace BioLinker.DTO.LinkDTO
{
    public class LinkCreate
    {
        public string? BioPageId { get; set; }
        public string? StaticLinkId { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public string? Platform { get; set; }
        public string? LinkType { get; set; } = "dynamic";
    }
}
