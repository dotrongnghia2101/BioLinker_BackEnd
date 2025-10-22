using BioLinker.DTO.LinkDTO;
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

        public async Task<IEnumerable<AnalyticLinkResponse>> GetAnalyticsByStaticLinkAsync(string staticLinkId)
        {
            var analytics = await _repo.GetByStaticLinkIdAsync(staticLinkId);

            return analytics.Select(a => new AnalyticLinkResponse
            {
                AnalyticsId = a.AnalyticsId,
                StaticLinkId = a.StaticLinkId,
                Views = a.Views,
                Clicks = a.Clicks,
                Date = a.Date
            }).ToList();
        }

        public async Task<int?> GetTotalClicksByStaticLinkAsync(string staticLinkId)
        {
            return await _repo.GetTotalClicksByStaticLinkAsync(staticLinkId);
        }

        public async Task<int?> GetTotalViewsByStaticLinkAsync(string staticLinkId)
        {
            return await _repo.GetTotalViewsByStaticLinkAsync(staticLinkId);
        }

        public async Task NotifyRealtimeClickAsync(string staticLinkId, int totalClicks)
        {
            await _hub.Clients.Group($"static_{staticLinkId}")
                 .SendAsync("ReceiveClickUpdate", new
                 {
                     staticLinkId,
                     totalClicks,
                     timestamp = DateTime.UtcNow
                 });
        }

        public async Task RecordClickAsync(string staticLinkId)
        {
            await _repo.AddOrUpdateClickAsync(staticLinkId);

            var total = await _repo.GetTotalClicksByStaticLinkAsync(staticLinkId);

            await _hub.Clients.Group($"static_{staticLinkId}")
                .SendAsync("ReceiveClickUpdate", new
                {
                    staticLinkId,
                    totalClicks = total,
                    timestamp = DateTime.UtcNow
                });
        }

        public async Task RecordViewAsync(string staticLinkId)
        {
            await _repo.AddOrUpdateViewAsync(staticLinkId);

            var total = await _repo.GetTotalViewsByStaticLinkAsync(staticLinkId);

            await _hub.Clients.Group($"static_{staticLinkId}")
                .SendAsync("ReceiveViewUpdate", new
                {
                    staticLinkId,
                    totalViews = total,
                    timestamp = DateTime.UtcNow
                });
        }
    }
}
