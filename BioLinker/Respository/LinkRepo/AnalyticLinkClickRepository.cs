using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.LinkRepo
{
    public class AnalyticLinkClickRepository : IAnalyticLinkClickRepository
    {
        private readonly AppDBContext _context;

        public AnalyticLinkClickRepository(AppDBContext context)
        {
            _context = context;
        }

        // Them 1 record moi vao bang chi tiet click
        public async Task AddAsync(string staticLinkId)
        {
            var click = new AnalyticLinkClick
            {
                StaticLinkId = staticLinkId,
                CreatedAt = DateTime.UtcNow
            };

            _context.AnalyticLinkClicks.Add(click);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AnalyticLinkClick>> GetAllAsync()
        {
            return await _context.AnalyticLinkClicks
                .Include(c => c.StaticLink)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // Lay danh sach click theo StaticLink
        public async Task<IEnumerable<AnalyticLinkClick>> GetByStaticLinkAsync(string staticLinkId, DateTime? fromUtc = null, DateTime? toUtc = null)
        {
            var query = _context.AnalyticLinkClicks
                .AsNoTracking()
                .Where(x => x.StaticLinkId == staticLinkId);

            if (fromUtc.HasValue)
                query = query.Where(x => x.CreatedAt >= fromUtc.Value);

            if (toUtc.HasValue)
                query = query.Where(x => x.CreatedAt <= toUtc.Value);

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<AnalyticLinkClick>> GetByUserIdAsync(string userId)
        {
            return await _context.AnalyticLinkClicks
              .Include(c => c.StaticLink)
              .Where(c => c.StaticLink.UserId == userId)
              .OrderByDescending(c => c.CreatedAt)
              .ToListAsync();
        }
    }
}
