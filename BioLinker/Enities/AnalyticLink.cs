namespace BioLinker.Enities
{
    public class AnalyticLink
    {
        public string? AnalyticsId { get; set; } = Guid.NewGuid().ToString();// PK
        public string? LinkId { get; set; } // FK
        public string? BioPageId { get; set; } // FK
        public int? Views { get; set; }
        public int? Clicks { get; set; }
        public DateTime Date { get; set; }
    }
}
