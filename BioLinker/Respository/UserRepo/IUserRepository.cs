using EntityUser = BioLinker.Enities.User;

namespace BioLinker.Respository.UserRepo
{
    public interface IUserRepository
    {
        Task<EntityUser?> GetByEmailAsync(string email);
        Task<IEnumerable<EntityUser>> GetAllUsersAsync();
        Task<EntityUser?> GetByIdAsync(string userId);
        Task AddUserAsync(EntityUser user);
        Task UpdateAsync(EntityUser user);
        // Lay tat ca custom domain khong null
        Task<List<string>> GetAllCustomDomainNamesAsync();

        // Lay custom domain theo userId
        Task<string?> GetCustomDomainByUserIdAsync(string userId);

        Task<List<string>> GetAllEmailsAsync();
    }
}
