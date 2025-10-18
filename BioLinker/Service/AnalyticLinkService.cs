using BioLinker.Enities;
using BioLinker.Helper;
using BioLinker.Respository.LinkRepo;
using Microsoft.AspNetCore.SignalR;

namespace BioLinker.Service
{
    public class AnalyticLinkService : IAnalyticLinkService
    {

        private readonly IAnalyticLinkRepository _repo;
        private readonly IHubContext<AnalyticsHub> _hub;

        public AnalyticLinkService(IAnalyticLinkRepository repo, IHubContext<AnalyticsHub> hub)
        {
            _repo = repo;
            _hub = hub;
        }

        public async Task RecordClickAsync(string linkId)
        {
            await _repo.AddOrUpdateClickAsync(linkId);

            // lay tong click moi
            var total = await _repo.GetTotalClicksByLinkAsync(linkId);

            // Gửi tín hiệu realtime cho tất cả client thuộc BioPage đó
            await _hub.Clients.All.SendAsync("ReceiveClickUpdate", new
            {
                linkId = linkId,
                totalClicks = total,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task<IEnumerable<AnalyticLink>> GetAnalyticsByLinkAsync(string linkId)
        {
            return await _repo.GetByLinkIdAsync(linkId);
        }

        public async Task<int?> GetTotalClicksByLinkAsync(string linkId)
        {
            return await _repo.GetTotalClicksByLinkAsync(linkId);
        }

        public async Task<int?> GetTotalClicksByPageAsync(string bioPageId)
        {
            return await _repo.GetTotalClicksByPageAsync(bioPageId);
        }

        public async Task RecordViewAsync(string bioPageId)
        {
            await _repo.AddOrUpdatePageViewAsync(bioPageId);

            
            await _hub.Clients.Group($"bio_{bioPageId}")
                .SendAsync("ReceiveViewUpdate", new
                {
                    bioPageId = bioPageId,
                    time = DateTime.UtcNow
                });
        }

        public async Task NotifyRealtimeClickAsync(string linkId, int totalClicks)
        {
            await _hub.Clients.All.SendAsync("ReceiveClickUpdate", new
            {
                linkId = linkId,
                totalClicks = totalClicks,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
