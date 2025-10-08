namespace BioLinker.DTO
{
    public class FacebookLoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string? UserImage { get; set; } = string.Empty;
    }
}
