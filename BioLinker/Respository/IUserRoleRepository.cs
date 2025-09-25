using BioLinker.Enities;

namespace BioLinker.Respository
{
    public interface IUserRoleRepository
    {
        Task AddAsync(UserRole userRole);
        Task<UserRole?> GetByIdAsync(string userRoleId);
    }
}
