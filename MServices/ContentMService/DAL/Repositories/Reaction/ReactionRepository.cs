using DAL.Repositories.Interfaces.Reaction;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Reaction
{
    public class ReactionRepository : BaseRepository<Entities.Reaction>, IReactionRepository
    {
        public ReactionRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Entities.Reaction>> GetByPostIdAsync(Guid postId)
        {
            return await _dbSet
                .Where(r => r.PostId == postId)
                .ToListAsync();
        }

        public async Task<Entities.Reaction?> GetByUserAndPostAsync(Guid userId, Guid postId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.UserId == userId && r.PostId == postId);
        }

        public async Task<IEnumerable<Entities.Reaction>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<int> GetCountByPostAsync(Guid postId)
        {
            return await _dbSet.CountAsync(r => r.PostId == postId);
        }

        public async Task<Dictionary<string, int>> GetReactionStatsAsync(Guid postId)
        {
            return await _dbSet
                .Where(r => r.PostId == postId)
                .GroupBy(r => r.Type)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type.ToString(), x => x.Count);
        }

        public async Task<bool> UserHasReactedAsync(Guid userId, Guid postId)
        {
            return await _dbSet.AnyAsync(r => r.UserId == userId && r.PostId == postId);
        }

        public async Task<bool> RemoveByUserAndPostAsync(Guid userId, Guid postId)
        {
            var reaction = await GetByUserAndPostAsync(userId, postId);
            if (reaction == null)
                return false;

            _dbSet.Remove(reaction);
            return true;
        }

        public override async Task AddAsync(Entities.Reaction entity)
        {
            var existing = await GetByUserAndPostAsync(entity.UserId, entity.PostId);
            if (existing != null)
            {
                existing = entity with { Id = existing.Id };
                Update(existing);
            }
            else
            {
                await base.AddAsync(entity);
            }
        }
    }
}