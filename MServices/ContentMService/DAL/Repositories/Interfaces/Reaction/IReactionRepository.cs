namespace DAL.Repositories.Interfaces.Reaction
{
    public interface IReactionRepository : IBaseRepository<Entities.Reaction>
    {
        public Task<IEnumerable<Entities.Reaction>> GetByPostIdAsync(Guid postId);
        public Task<Entities.Reaction?> GetByUserAndPostAsync(Guid userId, Guid postId);
        public Task<IEnumerable<Entities.Reaction>> GetByUserIdAsync(Guid userId);
        public Task<int> GetCountByPostAsync(Guid postId);
        public Task<Dictionary<string, int>> GetReactionStatsAsync(Guid postId);
        public Task<bool> UserHasReactedAsync(Guid userId, Guid postId);
        public Task<bool> RemoveByUserAndPostAsync(Guid userId, Guid postId);
    }
}