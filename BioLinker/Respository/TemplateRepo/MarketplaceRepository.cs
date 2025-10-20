using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.TemplateRepo
{
    public class MarketplaceRepository : IMarketplaceRepository
    {
        private readonly AppDBContext _db;
        public MarketplaceRepository(AppDBContext db)
        {
            _db = db;
        }
        public async Task AddAsync(Marketplace entity)
        {
            _db.Markets.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Marketplace entity)
        {
            _db.Markets.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Marketplace>> GetAllAsync()
        {
            return await _db.Markets
                .Include(m => m.Template)
                .Include(m => m.Seller)
                .OrderByDescending(m => m.SalesCount)
                .ToListAsync();
        }

        public async Task<Marketplace?> GetByIdAsync(string id)
        {
            return await _db.Markets
                .Include(m => m.Template)
                .Include(m => m.Seller)
                .FirstOrDefaultAsync(m => m.MarketplaceId == id);
        }

        public async Task<IEnumerable<Marketplace>> GetBySellerAsync(string sellerId)
        {
            return await _db.Markets
               .Include(m => m.Template)
               .Where(m => m.SellerId == sellerId)
               .ToListAsync();
        }

        public async Task<Marketplace?> GetByTemplateAsync(string templateId)
        {
            return await _db.Markets
                .FirstOrDefaultAsync(m => m.TemplateId == templateId); 
        }

        public async Task UpdateAsync(Marketplace entity)
        {
            _db.Markets.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
