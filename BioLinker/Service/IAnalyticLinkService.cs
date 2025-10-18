using BioLinker.Enities;

namespace BioLinker.Service
{
    public interface IAnalyticLinkService
    {
        Task RecordClickAsync(string linkId);
        Task RecordViewAsync(string bioPageId); // optional
        Task<IEnumerable<AnalyticLink>> GetAnalyticsByLinkAsync(string linkId);
        Task<int?> GetTotalClicksByLinkAsync(string linkId);
        Task<int?> GetTotalClicksByPageAsync(string bioPageId);
        Task NotifyRealtimeClickAsync(string linkId, int totalClicks);
    }
}
