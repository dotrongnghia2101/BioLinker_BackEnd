using BioLinker.Enities;

namespace BioLinker.Respository.LinkRepo
{
    public interface IAnalyticLinkRepository
    {
        Task AddOrUpdateClickAsync(string staticLinkId);
        Task AddOrUpdateViewAsync(string staticLinkId);
        Task<IEnumerable<AnalyticLink>> GetByStaticLinkIdAsync(string staticLinkId);
        Task<int?> GetTotalClicksByStaticLinkAsync(string staticLinkId);
        Task<int?> GetTotalViewsByStaticLinkAsync(string staticLinkId);

    }
}
