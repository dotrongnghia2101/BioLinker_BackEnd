namespace BioLinker.DTO
{
    public class ProfileCustomizeUpdate
    {
        public string UserId { get; set; } = default!;
        public string? Job { get; set; }
        public string? Nickname { get; set; }
        public string? Description { get; set; }
        public string? CustomDomain { get; set; }
        public string? UserImage { get; set; }
    }
}
