using BioLinker.Data;
using EntityUser = BioLinker.Enities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _context;
        public UserRepository(AppDBContext context)
        {
            _context = context;
        }
        public async Task AddUserAsync(EntityUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetAllCustomDomainNamesAsync()
        {
            return await _context.Users
                .Where(u => u.CustomerDomain != null && u.CustomerDomain != "")
                .Select(u => u.CustomerDomain!)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<EntityUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<EntityUser?> GetByEmailAsync(string email)
        {
            return await _context.Users
                        .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                        .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<EntityUser?> GetByIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<string?> GetCustomDomainByUserIdAsync(string userId)
        {
            return await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.CustomerDomain)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(EntityUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
