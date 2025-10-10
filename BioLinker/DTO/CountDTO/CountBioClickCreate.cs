namespace BioLinker.DTO.CountDTO
{
    public class CountBioClickCreate
    {
        public string? UserId { get; set; }
        public string? BioPageId { get; set; }
        public string? ClickedId { get; set; }
        public DateTime ClickedTime { get; set; }
    }
}
