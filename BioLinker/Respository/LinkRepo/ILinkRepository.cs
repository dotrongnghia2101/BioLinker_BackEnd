using BioLinker.Enities;

namespace BioLinker.Respository.LinkRepo
{
    public interface ILinkRepository
    {
        Task<IEnumerable<Link>> GetAllByBioPageAsync(string bioPageId);
        Task<Link?> GetByIdAsync(string linkId);
        Task<Link> CreateAsync(Link link);
        Task UpdateAsync(Link link);
        Task DeleteAsync(string linkId);
    }
}
