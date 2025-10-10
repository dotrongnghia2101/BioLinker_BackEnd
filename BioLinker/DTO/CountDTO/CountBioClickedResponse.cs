namespace BioLinker.DTO.CountDTO
{
    public class CountBioClickedResponse
    {
        public string? CountBioClickedId { get; set; }
        public string? UserId { get; set; }
        public string? BioPageId { get; set; }
        public string? ClickedId { get; set; }
        public DateTime ClickedTime { get; set; }
    }
}
