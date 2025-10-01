using BioLinker.Helper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Text.Json;
using Size = BioLinker.Helper.Size;

namespace BioLinker.Enities
{
    public class Content
    {
        public string? ContentId { get; set; } = Guid.NewGuid().ToString();
        public string? BioPageId { get; set; } //FK
        public string? ElementType { get; set; }   // avatar, name, title, bio, link...
        public string? Alignment { get; set; }     // left, center, right
        public bool Visible { get; set; }
        public string? PositionData { get; set; }  // JSON
        public string? SizeData { get; set; }      // JSON
        public string? StyleData { get; set; }     // JSON
        public string? ElementData { get; set; }   // JSON
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual BioPage? BioPage { get; set; }

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
