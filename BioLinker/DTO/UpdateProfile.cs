namespace BioLinker.DTO
{
    public class UpdateProfile
    {
        public string UserId { get; set; }
        public string? FullName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserImage { get; set; }
    }
}
