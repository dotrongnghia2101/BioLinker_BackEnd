using BioLinker.DTO.LinkDTO;

namespace BioLinker.Service
{
    public interface IStaticLinkService
    {
        Task<StaticLinkResponse?> GetAsync(string id);
        Task<IEnumerable<StaticLinkResponse>> GetByUserAsync(string userId);
        Task<StaticLinkResponse> CreateAsync(StaticLinkCreate dto);
        Task<bool> UpdateAsync(string id, string userId, StaticLinkUpdate dto);
        Task<bool> DeleteAsync(string id, string userId);
    }
}
