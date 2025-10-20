using BioLinker.Enities;

namespace BioLinker.Respository.TemplateRepo
{
    public interface IMarketplaceRepository
    {
        Task AddAsync(Marketplace entity);
        Task<IEnumerable<Marketplace>> GetAllAsync();
        Task<IEnumerable<Marketplace>> GetBySellerAsync(string sellerId);
        Task<Marketplace?> GetByIdAsync(string id);
        Task<Marketplace?> GetByTemplateAsync(string templateId);
        Task DeleteAsync(Marketplace entity);
        Task UpdateAsync(Marketplace entity);
    }
}
