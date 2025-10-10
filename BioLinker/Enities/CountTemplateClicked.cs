using System.ComponentModel.DataAnnotations;

namespace BioLinker.Enities
{
    public class CountTemplateClicked
    {
        [Key]
        public string CountTemplateClickedId { get; set; } = Guid.NewGuid().ToString(); // PK

        [Required]
        public string TemplateId { get; set; } = string.Empty; // FK -> Template

        [Required]
        public string ClickedId { get; set; } = string.Empty; // Id nguoi click (user hoac anonymous)

        public DateTime ClickedTime { get; set; } = DateTime.UtcNow; // Thoi gian click

        // Navigation
        public virtual Template? Template { get; set; }
    }
}
