using BioLinker.DTO.TemplateDTO;

namespace BioLinker.Service
{
    public interface IMarketplaceService
    {
        Task<MarketplaceResponse?> SellTemplateAsync(MarketplaceCreate dto);
        Task<IEnumerable<MarketplaceResponse>> GetAllAsync();
        Task<IEnumerable<MarketplaceResponse>> GetBySellerAsync(string sellerId);
        Task<bool> RemoveAsync(string id, string sellerId);
    }
}
