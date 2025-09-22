using System.ComponentModel.DataAnnotations;

namespace BioLinker.Enities
{
    public class UserRole
    {
        public string? RoleId { get; set; }
        public string? UserId { get; set; }
        [Key] public string? UserRoleId { get; set; } = Guid.NewGuid().ToString();
        public DateTime? StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }

        // Navigation
        public virtual User? User { get; set; }
        public virtual Role? Role { get; set; }
    }
}
