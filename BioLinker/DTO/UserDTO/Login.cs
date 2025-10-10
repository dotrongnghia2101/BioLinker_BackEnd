using System.ComponentModel.DataAnnotations;

namespace BioLinker.DTO.UserDTO
{
    public class Login
    {
        //login by email
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }
}
