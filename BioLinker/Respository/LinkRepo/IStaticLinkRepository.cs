using BioLinker.Enities;

namespace BioLinker.Respository.LinkRepo
{
    public interface IStaticLinkRepository
    {
        Task<StaticLink?> GetByIdAsync(string id);
        Task<IEnumerable<StaticLink>> GetByUserAsync(string userId);
        Task AddAsync(StaticLink entity);
        Task UpdateAsync(StaticLink entity);
        Task DeleteAsync(StaticLink entity);
    }
}
