using BioLinker.Data;
using BioLinker.Enities;
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
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                        .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                        .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
