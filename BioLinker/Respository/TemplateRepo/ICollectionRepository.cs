using BioLinker.Enities;

namespace BioLinker.Respository.TemplateRepo
{
    public interface ICollectionRepository
    {
        Task<Collection?> GetByUserIdAsync(string userId);
        Task AddAsync(Collection collection);
        Task UpdateAsync(Collection collection);
    }
}
