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

        Task<IEnumerable<AnalyticLinkClick>> GetClickDetailsAsync(string staticLinkId, DateTime? fromUtc = null, DateTime? toUtc = null);

        // ✅ Lấy toàn bộ lịch sử click theo UserId (tất cả link thuộc user)
        Task<IEnumerable<AnalyticLinkClick>> GetClickDetailsByUserAsync(string userId);

        // ✅ Lấy toàn bộ click (debug/test)
        Task<IEnumerable<AnalyticLinkClick>> GetAllClickAsync();
    }
}
