namespace BioLinker.DTO.TemplateDTO
{
    public class CollectionRequest
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> TemplateIds { get; set; } = new();
    }
}
