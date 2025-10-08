using System.ComponentModel.DataAnnotations;

namespace BioLinker.DTO
{
    public class Register
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; } = string.Empty;
        [Required] public string? Password { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
