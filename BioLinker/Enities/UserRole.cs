namespace BioLinker.Enities
{
    public class UserRole
    {
        public string? RoleId { get; set; }
        public string? UserId { get; set; }
        public string? UserRoleId { get; set; } = Guid.NewGuid().ToString();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
