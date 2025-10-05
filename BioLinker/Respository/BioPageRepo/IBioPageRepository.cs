using BioLinker.Enities;

namespace BioLinker.Respository.BioPageRepo
{
    public interface IBioPageRepository
    {
        Task<BioPage?> GetByIdAsync(string id);
        Task<IEnumerable<BioPage>> GetAllAsync();
        Task AddAsync(BioPage entity);
        Task UpdateAsync(BioPage entity);
        Task DeleteAsync(BioPage entity);
    }
}
