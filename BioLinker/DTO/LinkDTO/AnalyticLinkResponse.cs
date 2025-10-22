namespace BioLinker.DTO.LinkDTO
{
    public class AnalyticLinkResponse
    {
        public string? AnalyticsId { get; set; }
        public string? StaticLinkId { get; set; }
        public int? Views { get; set; }
        public int? Clicks { get; set; }
        public DateTime Date { get; set; }
    }
}
