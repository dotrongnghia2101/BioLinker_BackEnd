using BioLinker.Enities;

namespace BioLinker.Respository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task UpdateAsync(User user);
    }
}
