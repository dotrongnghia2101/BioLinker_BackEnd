using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDBContext _context;
        public UserRoleRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task<UserRole?> GetByIdAsync(string userRoleId)
        {
            return await _context.UserRoles
                                .Include(ur => ur.Role)
                                .Include(ur => ur.User)
                                .FirstOrDefaultAsync(ur => ur.UserRoleId == userRoleId);
        }
    }
}
