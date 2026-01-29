using DAL.Repositories.Interfaces.Post;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Post
{
    public class PostRepository : BaseRepository<Entities.Post>, IPostRepository
    {
        public PostRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Entities.Post>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entities.Post>> GetRecentAsync(int count)
        {
            return await _dbSet
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Entities.Post?> GetWithCommentsAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Entities.Post?> GetWithCommentsAndReactionsAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Comments)
                .Include(p => p.Reactions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(p => p.Id == id);
        }

        public async Task<int> GetCountByUserAsync(Guid userId)
        {
            return await _dbSet.CountAsync(p => p.UserId == userId);
        }

        public override async Task<IEnumerable<Entities.Post>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.Comments)
                .Include(p => p.Reactions)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Entities.Post?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Comments)
                .Include(p => p.Reactions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}