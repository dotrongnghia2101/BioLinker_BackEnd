using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.TemplateRepo
{
    public class CollectionRepository : ICollectionRepository
    {

        private readonly AppDBContext _context;

        public CollectionRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Collection collection)
        {
            await _context.Collections.AddAsync(collection);
            await _context.SaveChangesAsync();
        }

        public async Task<Collection?> GetByUserIdAsync(string userId)
        {
            return await _context.Collections.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task UpdateAsync(Collection collection)
        {
            _context.Collections.Update(collection);
            await _context.SaveChangesAsync();
        }
    }
}
