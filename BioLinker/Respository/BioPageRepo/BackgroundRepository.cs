using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.BioPageRepo
{
    public class BackgroundRepository : IBackgroundRepository
    {
        private readonly AppDBContext _context;
        public BackgroundRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task<Background> AddAsync(Background background)
        {
            _context.Backgrounds.Add(background);
            await _context.SaveChangesAsync();
            return background;
        }

        public async Task DeleteAsync(Background background)
        {
            _context.Backgrounds.Remove(background);
            await _context.SaveChangesAsync();
        }

        public async Task<Background?> GetByBioPageIdAsync(string bioPageId)
        {
            return await _context.Backgrounds.FirstOrDefaultAsync(b => b.BioPageId == bioPageId);
        }

        public async Task<Background?> GetByIdAsync(string id)
        {
            return await _context.Backgrounds.FirstOrDefaultAsync(b => b.BackgroundId == id);

        }

        public async Task UpdateAsync(Background background)
        {
            _context.Backgrounds.Update(background);
            await _context.SaveChangesAsync();
        }
    }
}
