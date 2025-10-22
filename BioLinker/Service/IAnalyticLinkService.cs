using BioLinker.DTO.LinkDTO;
using BioLinker.Enities;

namespace BioLinker.Service
{
    public interface IAnalyticLinkService
    {
        Task RecordClickAsync(string staticLinkId);
        Task RecordViewAsync(string staticLinkId);
        Task<IEnumerable<AnalyticLinkResponse>> GetAnalyticsByStaticLinkAsync(string staticLinkId);
        Task<int?> GetTotalClicksByStaticLinkAsync(string staticLinkId);
        Task<int?> GetTotalViewsByStaticLinkAsync(string staticLinkId);
        Task NotifyRealtimeClickAsync(string staticLinkId, int totalClicks);
    }
}
