using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.TemplateRepo
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly AppDBContext _context;

        public TemplateRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Template template)
        {
            await _context.Templates.AddAsync(template);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var template = await _context.Templates.FindAsync(id);
            if (template != null)
            {
                _context.Templates.Remove(template);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Template>> GetAllAsync()
        {
            return await _context.Templates.ToListAsync();
        }

        public async Task<Template?> GetByIdAsync(string id)
        {
            return await _context.Templates
                    .Include(t => t.Style)
                    .Include(t => t.Background)
                    .Include(t => t.StyleSettings)
                    .Include(t => t.TemplateDetails)
                    .FirstOrDefaultAsync(t => t.TemplateId == id);
        }

        public async Task UpdateAsync(Template template)
        {
            _context.Templates.Update(template);
            await _context.SaveChangesAsync();
        }
    }
}
