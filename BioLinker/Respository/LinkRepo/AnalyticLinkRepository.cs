using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.LinkRepo
{
    public class AnalyticLinkRepository : IAnalyticLinkRepository
    {
        private readonly AppDBContext _context;

        public AnalyticLinkRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateClickAsync(string staticLinkId)
        {
            var today = DateTime.UtcNow.Date;

            var analytic = await _context.AnalyticLinks
                .FirstOrDefaultAsync(a => a.StaticLinkId == staticLinkId && a.Date == today);

            if (analytic == null)
            {
                analytic = new AnalyticLink
                {
                    StaticLinkId = staticLinkId,
                    Clicks = 1,
                    Views = 0,
                    Date = today
                };
                _context.AnalyticLinks.Add(analytic);
            }
            else
            {
                analytic.Clicks += 1;
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateViewAsync(string staticLinkId)
        {
            var today = DateTime.UtcNow.Date;

            var analytic = await _context.AnalyticLinks
                .FirstOrDefaultAsync(a => a.StaticLinkId == staticLinkId && a.Date == today);

            if (analytic == null)
            {
                analytic = new AnalyticLink
                {
                    StaticLinkId = staticLinkId,
                    Clicks = 0,
                    Views = 1,
                    Date = today
                };
                _context.AnalyticLinks.Add(analytic);
            }
            else
            {
                analytic.Views += 1;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AnalyticLink>> GetByStaticLinkIdAsync(string staticLinkId)
        {
            return await _context.AnalyticLinks
                .Where(a => a.StaticLinkId == staticLinkId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<int?> GetTotalClicksByStaticLinkAsync(string staticLinkId)
        {
            return await _context.AnalyticLinks
               .Where(a => a.StaticLinkId == staticLinkId)
               .SumAsync(a => a.Clicks);
        }

        public async Task<int?> GetTotalViewsByStaticLinkAsync(string staticLinkId)
        {
            return await _context.AnalyticLinks
             .Where(a => a.StaticLinkId == staticLinkId)
             .SumAsync(a => a.Views);
        }
    }
}
