using BioLinker.Enities;

namespace BioLinker.Respository.LinkRepo
{
    public interface IAnalyticLinkRepository
    {
        Task AddOrUpdateClickAsync(string linkId);
        Task<IEnumerable<AnalyticLink>> GetByLinkIdAsync(string linkId);
        Task<int?> GetTotalClicksByLinkAsync(string linkId);
        Task<int?> GetTotalClicksByPageAsync(string bioPageId);
        Task AddOrUpdatePageViewAsync(string bioPageId);

    }
}
