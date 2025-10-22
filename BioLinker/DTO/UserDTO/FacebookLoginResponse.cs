namespace BioLinker.DTO.UserDTO
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
        public string? CurrentPlanId { get; set; } // luu ma goi hien tai (FREE-PLAN, PRO-PLAN, ...)
        public DateTime? PlanExpireAt { get; set; } // thoi gian het han goi
    }
}
