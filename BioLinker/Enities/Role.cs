using System.ComponentModel.DataAnnotations;

namespace BioLinker.Enities
{
    public class Role
    {
        [Key] public string? RoleId { get; set; } = Guid.NewGuid().ToString();
        public string? RoleName { get; set; }

        // Navigation
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
