using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.LinkRepo
{

    public class LinkRepository : ILinkRepository
    {
        private readonly AppDBContext _context;

        public LinkRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task<Link> CreateAsync(Link link)
        {
            _context.Links.Add(link);
            await _context.SaveChangesAsync();
            return link;
        }

        public async Task DeleteAsync(string linkId)
        {
            var link = await _context.Links.FindAsync(linkId);
            if (link != null)
            {
                _context.Links.Remove(link);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Link>> GetAllByBioPageAsync(string bioPageId)
        {
            return await _context.Links
              .Where(l => l.BioPageId == bioPageId)
              .OrderBy(l => l.Position)
              .ToListAsync();
        }

        public async Task<Link?> GetByIdAsync(string linkId)
        {
            return await _context.Links
               .FirstOrDefaultAsync(l => l.LinkId == linkId);
        }

        public async Task UpdateAsync(Link link)
        {
            _context.Links.Update(link);
            await _context.SaveChangesAsync();
        }
    }
}
