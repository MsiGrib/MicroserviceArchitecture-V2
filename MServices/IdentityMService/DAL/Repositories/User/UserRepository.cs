using DAL.Repositories.Interfaces.User;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.User
{
    public class UserRepository : BaseRepository<Entities.User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<Entities.User?> GetByEmailAsync(string email)
        {
            var query = _context.Users.AsQueryable();

            return await query.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Entities.User?> GetByUsernameAsync(string username)
        {
            var query = _context.Users.AsQueryable();

            return await query.FirstOrDefaultAsync(u => u.Username == username);
        }

        public new async Task<Entities.User?> GetByIdAsync(Guid id)
        {
            var query = _context.Users.AsQueryable();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
            => await _context.Users.AnyAsync(u => u.Email == email);

        public async Task<bool> ExistsByUsernameAsync(string username)
            => await _context.Users.AnyAsync(u => u.Username == username);
    }
}