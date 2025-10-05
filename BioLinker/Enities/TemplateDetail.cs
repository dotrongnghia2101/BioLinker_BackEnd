using BioLinker.Helper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BioLinker.Enities
{
    public class TemplateDetail
    {
        public string? TemplateDetailId { get; set; } = Guid.NewGuid().ToString(); // PK
        public string? TemplateId { get; set; } // FK -> Template

        public string? ElementType { get; set; }   // name, title, bio, link...
        public string? PositionData { get; set; }  // JSON: toạ độ, layout
        public string? SizeData { get; set; }      // JSON: kích thước
        public string? StyleData { get; set; }     // JSON: style button, text, color
        public string? ElementData { get; set; }   // JSON: content mặc định
        public int OrderIndex { get; set; }        // thứ tự hiển thị trong template
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Navigation
        public virtual Template? Template { get; set; }

        // --------- Helpers (NotMapped) ---------
        [NotMapped]
        public Position? Position
        {
            get => string.IsNullOrEmpty(PositionData) ? null
                : JsonSerializer.Deserialize<Position>(PositionData);
            set => PositionData = JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public Size? Size
        {
            get => string.IsNullOrEmpty(SizeData) ? null
                : JsonSerializer.Deserialize<Size>(SizeData);
            set => SizeData = JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public StyleConfig? Style
        {
            get => string.IsNullOrEmpty(StyleData) ? null
                : JsonSerializer.Deserialize<StyleConfig>(StyleData);
            set => StyleData = JsonSerializer.Serialize(value);
        }

        [NotMapped]
        public ElementContent? Element
        {
            get => string.IsNullOrEmpty(ElementData) ? null
                : JsonSerializer.Deserialize<ElementContent>(ElementData);
            set => ElementData = JsonSerializer.Serialize(value);
        }
    }
}
