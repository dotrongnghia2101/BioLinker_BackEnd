using BioLinker.DTO.TemplateDTO;
using BioLinker.Enities;
using BioLinker.Respository.TemplateRepo;

namespace BioLinker.Service
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IMarketplaceRepository _repo;

        public MarketplaceService(IMarketplaceRepository repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<MarketplaceResponse>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();
            return data.Select(x => new MarketplaceResponse
            {
                MarketplaceId = x.MarketplaceId,
                TemplateId = x.TemplateId,
                TemplateName = x.Template?.Name,
                SellerId = x.SellerId,
                SellerName = x.Seller?.NickName,
                Price = x.Price,
                SalesCount = x.SalesCount
            });
        }

        public async Task<IEnumerable<MarketplaceResponse>> GetBySellerAsync(string sellerId)
        {
            var data = await _repo.GetBySellerAsync(sellerId);
            return data.Select(x => new MarketplaceResponse
            {
                MarketplaceId = x.MarketplaceId,
                TemplateId = x.TemplateId,
                TemplateName = x.Template?.Name,
                SellerId = x.SellerId,
                SellerName = x.Seller?.NickName,
                Price = x.Price,
                SalesCount = x.SalesCount
            });
        }

        public async Task<bool> RemoveAsync(string id, string sellerId)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.SellerId != sellerId)
                return false;

            await _repo.DeleteAsync(entity);
            return true;
        }

        public async Task<MarketplaceResponse?> SellTemplateAsync(MarketplaceCreate dto)
        {
            if (dto.TemplateId == null || dto.SellerId == null)
                return null;

            // Kiểm tra template đã được bán chưa
            var exists = await _repo.GetByTemplateAsync(dto.TemplateId);
            if (exists != null)
                throw new InvalidOperationException("Template is post for sale");

            var entity = new Marketplace
            {
                TemplateId = dto.TemplateId,
                SellerId = dto.SellerId,
                Price = dto.Price
            };

            await _repo.AddAsync(entity);

            return new MarketplaceResponse
            {
                MarketplaceId = entity.MarketplaceId,
                TemplateId = entity.TemplateId,
                SellerId = entity.SellerId,
                Price = entity.Price,
                SalesCount = entity.SalesCount
            };
        }
    }
}
