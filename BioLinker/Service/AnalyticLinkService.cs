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
        private readonly IAnalyticLinkClickRepository _clickRepo;

        public AnalyticLinkService(
            IAnalyticLinkRepository repo,
            IAnalyticLinkClickRepository clickRepo,
            IHubContext<AnalyticsHub> hub)
        {
            _repo = repo;
            _clickRepo = clickRepo;
            _hub = hub;
        }

        public async Task<IEnumerable<AnalyticLinkClick>> GetAllClickAsync()
        {
            return await _clickRepo.GetAllAsync();
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

        public async Task<IEnumerable<AnalyticLinkClick>> GetClickDetailsAsync(string staticLinkId, DateTime? fromUtc = null, DateTime? toUtc = null)
        {
            return await _clickRepo.GetByStaticLinkAsync(staticLinkId, fromUtc, toUtc);
        }

        public async Task<IEnumerable<AnalyticLinkClick>> GetClickDetailsByUserAsync(string userId)
        {
            return await _clickRepo.GetByUserIdAsync(userId);
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

        // Cap nhat tong + luu chi tiet click
        public async Task RecordClickAsync(string staticLinkId)
        {
            // (1) Cap nhat bang tong hop theo ngay
            await _repo.AddOrUpdateClickAsync(staticLinkId);

            // (2) Luu chi tiet vao bang AnalyticLinkClick
            await _clickRepo.AddAsync(staticLinkId);

            var total = await _repo.GetTotalClicksByStaticLinkAsync(staticLinkId);

            // (3) Gui realtime (neu co client ket noi)
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
