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
    }
}
