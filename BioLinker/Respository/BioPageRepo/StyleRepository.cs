using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.BioPageRepo
{
    public class StyleRepository : IStyleRepository
    {
        private readonly AppDBContext _context;
        public StyleRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Style style)
        {
            _context.Styles.Add(style);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var style = await _context.Styles.FindAsync(id);
            if (style != null)
            {
                _context.Styles.Remove(style);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Style>> GetAllAsync()
        {
            return await _context.Styles.ToListAsync();
        }

        public async Task<Style?> GetByIdAsync(string id)
        {
            return await _context.Styles.FindAsync(id);
        }

        public async Task UpdateAsync(Style style)
        {
            _context.Styles.Update(style);
            await _context.SaveChangesAsync();
        }
    }
}
