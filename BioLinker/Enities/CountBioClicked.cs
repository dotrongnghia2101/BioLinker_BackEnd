using System.ComponentModel.DataAnnotations;

namespace BioLinker.Enities
{
    public class CountBioClicked
    {
        [Key]
        public string CountBioClickedId { get; set; } = Guid.NewGuid().ToString(); // PK

        [Required]
        public string UserId { get; set; } = string.Empty; // FK -> User

        [Required]
        public string BioPageId { get; set; } = string.Empty; // FK -> BioPage (trang duoc click)

        [Required]
        public string ClickedId { get; set; } = string.Empty; // Id nguoi click (user hoac anonymous)

        public DateTime ClickedTime { get; set; } = DateTime.UtcNow; // Thoi gian click

        // Navigation
        public virtual User? User { get; set; }
        public virtual BioPage? BioPage { get; set; }
    }
}
