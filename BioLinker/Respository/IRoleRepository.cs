using BioLinker.Enities;

namespace BioLinker.Respository
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(string roleId);
        Task<Role?> GetByNameAsync(string roleName);
        Task<IEnumerable<Role>> GetAllAsync();
    }
}
