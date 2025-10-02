using BioLinker.Enities;

namespace BioLinker.Respository.BioPageRepo
{
    public interface IStyleRepository
    {
        Task<IEnumerable<Style>> GetAllAsync();
        Task<Style?> GetByIdAsync(string id);
        Task AddAsync(Style style);
        Task UpdateAsync(Style style);
        Task DeleteAsync(string id);
    }
}
