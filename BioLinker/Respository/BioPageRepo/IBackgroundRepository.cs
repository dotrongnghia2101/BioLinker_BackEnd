using BioLinker.Enities;

namespace BioLinker.Respository.BioPageRepo
{
    public interface IBackgroundRepository
    {
        Task<Background?> GetByIdAsync(string id);
        Task<Background?> GetByBioPageIdAsync(string bioPageId);
        Task<Background> AddAsync(Background background);
        Task UpdateAsync(Background background);
        Task DeleteAsync(Background background);
    }
}
