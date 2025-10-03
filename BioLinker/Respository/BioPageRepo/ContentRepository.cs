using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.BioPageRepo
{
    public class ContentRepository : IContentRepository
    {

        private readonly AppDBContext _context;
        public ContentRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task<Content> AddAsync(Content content)
        {
            _context.Contents.Add(content);
            await _context.SaveChangesAsync();
            return content;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content == null) return false;

            _context.Contents.Remove(content);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Content>> GetByBioPageAsync(string bioPageId)
        {
            return await _context.Contents
                .Where(c => c.BioPageId == bioPageId)
                .ToListAsync();
        }

        public async Task<Content?> GetByIdAsync(string id)
        {
           return await _context.Contents.FindAsync(id);
        }

        public async Task<Content> UpdateAsync(Content content)
        {
            _context.Contents.Update(content);
            await _context.SaveChangesAsync();
            return content;
        }
    }
}
