using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BioLinker.Enities
{
    public class Collection
    {
        public string? CollectionId { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public string TemplateIdsJson { get; set; } = "[]";


        [NotMapped]
        public List<string> TemplateIds
        {
            get => string.IsNullOrEmpty(TemplateIdsJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(TemplateIdsJson) ?? new List<string>();
            set => TemplateIdsJson = JsonSerializer.Serialize(value);
        }
    }
}
