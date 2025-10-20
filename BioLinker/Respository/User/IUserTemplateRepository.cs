using BioLinker.Enities;

namespace BioLinker.Respository.User
{
    public interface IUserTemplateRepository
    {
        Task AddAsync(UserTemplate entity);
        Task<IEnumerable<UserTemplate>> GetByUserAsync(string userId);
        Task<UserTemplate?> GetByIdAsync(string id);
        Task<bool> ExistsAsync(string userId, string templateId);
    }
}
