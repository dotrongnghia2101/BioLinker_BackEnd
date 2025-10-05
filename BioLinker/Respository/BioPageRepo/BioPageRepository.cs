using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.BioPageRepo
{
    public class BioPageRepository : IBioPageRepository
    {

        private readonly AppDBContext _context;
        public BioPageRepository(AppDBContext context)
        {
            _context = context;
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
           return await _context.BioPages.ToListAsync();
        }

        public async Task<BioPage?> GetByIdAsync(string id)
        {
            return await _context.BioPages
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
    }
}
