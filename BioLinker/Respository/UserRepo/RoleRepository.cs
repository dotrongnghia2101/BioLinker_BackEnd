using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.UserRepo
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDBContext _context;

        public RoleRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(string roleId)
        {
            return await _context.Roles
                              .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _context.Roles
                             .FirstOrDefaultAsync(r => r.RoleName == roleName);
        }
    }
}
