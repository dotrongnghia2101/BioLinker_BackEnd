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

        //cong don so click cho link hom nay
        public async Task AddOrUpdateClickAsync(string linkId)
        {
            var today = DateTime.UtcNow.Date;

            var analytic = await _context.AnalyticLinks
                .FirstOrDefaultAsync(a => a.LinkId == linkId && a.Date == today);

            if (analytic == null)
            {
                analytic = new AnalyticLink
                {
                    LinkId = linkId,
                    Clicks = 1,
                    Views = 0,
                    Date = today
                };
                _context.AnalyticLinks.Add(analytic);
            }
            else
            {
                analytic.Clicks += 1;
                _context.AnalyticLinks.Update(analytic);
            }

            await _context.SaveChangesAsync();
        }


        public async Task  AddOrUpdatePageViewAsync(string bioPageId)
        {
            var today = DateTime.UtcNow.Date;

            // Lấy tất cả link thuộc BioPage này
            var linkIds = await _context.Links
                .Where(l => l.BioPageId == bioPageId)
                .Select(l => l.LinkId)
                .ToListAsync();

            foreach (var linkId in linkIds)
            {
                var analytic = await _context.AnalyticLinks
                    .FirstOrDefaultAsync(a => a.LinkId == linkId && a.Date == today);

                if (analytic == null)
                {
                    _context.AnalyticLinks.Add(new AnalyticLink
                    {
                        AnalyticsId = Guid.NewGuid().ToString(),
                        LinkId = linkId,
                        Views = 1,
                        Clicks = 0,
                        Date = today
                    });
                }
                else
                {
                    analytic.Views += 1;
                    _context.AnalyticLinks.Update(analytic);
                }
            }

            await _context.SaveChangesAsync();
        }

        //lay toan bo analytic cua link 
        public async Task<IEnumerable<AnalyticLink>> GetByLinkIdAsync(string linkId)
        {
            return await _context.AnalyticLinks
           .Where(a => a.LinkId == linkId)
           .OrderByDescending(a => a.Date)
           .ToListAsync();
        }

        //lay tong click
        public async Task<int?> GetTotalClicksByLinkAsync(string linkId)
        {
            return await _context.AnalyticLinks
            .Where(a => a.LinkId == linkId)
            .SumAsync(a => a.Clicks);
        }

        //tong so click cua 1 bio
        public async Task<int?> GetTotalClicksByPageAsync(string bioPageId)
        {
            return await _context.AnalyticLinks
           .Include(a => a.Link)
           .Where(a => a.Link.BioPageId == bioPageId)
           .SumAsync(a => a.Clicks);
        }
    }
}
