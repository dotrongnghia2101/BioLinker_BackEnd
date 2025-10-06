using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.LinkRepo
{
    public class StaticLinkRepository : IStaticLinkRepository
    {
        private readonly AppDBContext _db;
        public StaticLinkRepository(AppDBContext db)
        {
            _db = db;
        }

        public async Task AddAsync(StaticLink entity)
        {
            _db.StaticLinks.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(StaticLink entity)
        {
            _db.StaticLinks.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<StaticLink?> GetByIdAsync(string id)
        {
             return await _db.StaticLinks.AsNoTracking().FirstOrDefaultAsync(x => x.StaticLinkId == id);
        }

        public async Task<IEnumerable<StaticLink>> GetByUserAsync(string userId)
        {
            return await _db.StaticLinks
                        .AsNoTracking()
                        .Where(x => x.UserId == userId)
                        .OrderByDescending(x => x.StaticLinkId)
                        .ToListAsync();
        }

        public async Task UpdateAsync(StaticLink entity)
        {
            _db.StaticLinks.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
