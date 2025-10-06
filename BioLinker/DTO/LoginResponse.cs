namespace BioLinker.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? UserImage { get; set; }
        public string? Job { get; set; }

    }
}
