using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.User
{
    public class UserTemplateRepository : IUserTemplateRepository
    {
        private readonly AppDBContext _db;
        public UserTemplateRepository(AppDBContext db)
        {
            _db = db;
        }

        public async Task AddAsync(UserTemplate entity)
        {
            _db.UserTemplates.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string userId, string templateId)
        {
            return await _db.UserTemplates
                .AnyAsync(x => x.UserId == userId && x.TemplateId == templateId);
        }

        public async Task<UserTemplate?> GetByIdAsync(string id)
        {
            return await _db.UserTemplates
                .Include(u => u.Template)
                .FirstOrDefaultAsync(x => x.UTemplateId == id);
        }

        public async Task<IEnumerable<UserTemplate>> GetByUserAsync(string userId)
        {
            return await _db.UserTemplates
                .Include(u => u.Template)
                .Where(u => u.UserId == userId)
                .OrderByDescending(u => u.PurchaseAt)
                .ToListAsync();
        }
    }
}
