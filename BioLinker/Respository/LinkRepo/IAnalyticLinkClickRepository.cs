using BioLinker.Enities;

namespace BioLinker.Respository.LinkRepo
{
    public interface IAnalyticLinkClickRepository
    {
        Task AddAsync(string staticLinkId);
        Task<IEnumerable<AnalyticLinkClick>> GetByStaticLinkAsync(string staticLinkId, DateTime? fromUtc = null, DateTime? toUtc = null);
        Task<IEnumerable<AnalyticLinkClick>> GetByUserIdAsync(string userId);
        Task<IEnumerable<AnalyticLinkClick>> GetAllAsync();
    }
}
