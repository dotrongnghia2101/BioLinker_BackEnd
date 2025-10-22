namespace BioLinker.DTO.UserDTO
{
    public class UserProfileResponse
    {
        public string UserId { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? UserImage { get; set; }
        public string? Job { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? IsGoogle { get; set; }
        public string? CustomerDomain { get; set; }
        public string? Description { get; set; }
        public string? NickName { get; set; }
        public bool? IsBeginner { get; set; }
        public string? BackgroundImage { get; set; }
        public string? CurrentPlanId { get; set; } // luu ma goi hien tai (FREE-PLAN, PRO-PLAN, ...)
        public DateTime? PlanExpireAt { get; set; } // thoi gian het han goi
    }
}
