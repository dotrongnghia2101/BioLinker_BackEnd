using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BioLinker.Respository.BioPageRepo
{
    public class BioPageRepository : IBioPageRepository
    {

        private readonly AppDBContext _context;
        public BioPageRepository(AppDBContext context)
        {
            _context = context;
        }

        // === Transaction Support ===
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task AddAsync(BioPage entity)
        {
            await _context.BioPages.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BioPage entity)
        {
            _context.BioPages.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BioPage>> GetAllAsync()
        {
            return await _context.BioPages
                 .AsNoTracking()
                 .Include(x => x.Style)
                 .Include(x => x.Background)
                 .Include(x => x.StyleSettings)
                 .Include(x => x.Contents)
                 .ToListAsync();
        }

        public async Task<BioPage?> GetByIdAsync(string id)
        {
            return await _context.BioPages
                .AsNoTracking()
                .Include(x => x.Style)
                .Include(x => x.Background)
                .Include(x => x.StyleSettings)
                .Include(x => x.Contents)
                .FirstOrDefaultAsync(x => x.BioPageId == id);
        }

        public async Task UpdateAsync(BioPage entity)
        {
            _context.BioPages.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BioPage>> GetByUserIdAsync(string userId)
        {
            return await _context.BioPages
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.Template)
                .Include(x => x.Style)
                .Include(x => x.Background)
                .Include(x => x.StyleSettings)
                .Include(x => x.Contents)
                .ToListAsync();
        }
    }
}
