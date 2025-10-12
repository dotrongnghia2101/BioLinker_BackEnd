using BioLinker.Enities;

namespace BioLinker.Respository.UserRepo
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetByIdAsync(string userId);
        Task AddUserAsync(User user);
        Task UpdateAsync(User user);
        // Lay tat ca custom domain khong null
        Task<List<string>> GetAllCustomDomainNamesAsync();

        // Lay custom domain theo userId
        Task<string?> GetCustomDomainByUserIdAsync(string userId);
    }
}
