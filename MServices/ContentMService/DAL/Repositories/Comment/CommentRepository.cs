using DAL.Repositories.Interfaces.Comment;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Comment
{
    public class CommentRepository : BaseRepository<Entities.Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Entities.Comment>> GetByPostIdAsync(Guid postId)
        {
            return await _dbSet
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.PostId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entities.Comment>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.PostId)
                .ToListAsync();
        }

        public async Task<int> GetCountByPostAsync(Guid postId)
        {
            return await _dbSet.CountAsync(c => c.PostId == postId);
        }

        public async Task<bool> UserOwnsCommentAsync(Guid commentId, Guid userId)
        {
            return await _dbSet.AnyAsync(c => c.Id == commentId && c.UserId == userId);
        }

        public override async Task<IEnumerable<Entities.Comment>> GetAllAsync()
        {
            return await _dbSet
                .OrderByDescending(c => c.PostId)
                .ToListAsync();
        }
    }
}