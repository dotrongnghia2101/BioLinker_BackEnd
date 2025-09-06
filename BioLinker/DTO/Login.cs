using System.ComponentModel.DataAnnotations;

namespace BioLinker.DTO
{
    public class Login
    {
        //login by email
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email must be a Gmail address")]
        public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }
}
