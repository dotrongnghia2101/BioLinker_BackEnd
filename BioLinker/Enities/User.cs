using System.ComponentModel.DataAnnotations;

namespace BioLinker.Enities
{
    public class User
    {
        [Key] public String UserId { get; set; } = Guid.NewGuid().ToString();

        public bool? IsActive { get; set; }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? Role { get; set; }
        public string? LastName { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserImage { get; set; }
        public string? PasswordHash { get; set; }
    }
}
