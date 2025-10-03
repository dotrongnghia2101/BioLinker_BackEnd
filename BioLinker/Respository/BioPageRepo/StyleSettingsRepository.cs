using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.BioPageRepo
{
    public class StyleSettingsRepository : IStyleSettingsRepository
    {
        private readonly AppDBContext _context;

        public StyleSettingsRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<StyleSettings> CreateAsync(StyleSettings settings)
        {
            _context.StyleSettings.Add(settings);
            await _context.SaveChangesAsync();
            return settings;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var existing = await _context.StyleSettings.FindAsync(id);
            if (existing == null) return false;

            _context.StyleSettings.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<StyleSettings?> GetByBioPageIdAsync(string bioPageId)
        {
            return await _context.StyleSettings
                                 .FirstOrDefaultAsync(s => s.BioPageId == bioPageId);
        }

        public async Task<StyleSettings?> GetByIdAsync(string id)
        {
            return await _context.StyleSettings.FindAsync(id);
        }

        public async Task<StyleSettings?> UpdateAsync(string id, StyleSettings settings)
        {
            var existing = await _context.StyleSettings.FindAsync(id);
            if (existing == null) return null;

            existing.Thumbnail = settings.Thumbnail ?? existing.Thumbnail;
            existing.MetaTitle = settings.MetaTitle ?? existing.MetaTitle;
            existing.MetaDescription = settings.MetaDescription ?? existing.MetaDescription;
            existing.CookieBanner = settings.CookieBanner ?? existing.CookieBanner;

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
