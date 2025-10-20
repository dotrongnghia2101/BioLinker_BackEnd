using BioLinker.Enities;

namespace BioLinker.Respository.UserRepo
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(string roleId);
        Task<Role?> GetByNameAsync(string roleName);
        Task<IEnumerable<Role>> GetAllAsync();
    }
}
