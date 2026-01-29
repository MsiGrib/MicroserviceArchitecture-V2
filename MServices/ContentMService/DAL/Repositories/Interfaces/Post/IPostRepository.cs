namespace DAL.Repositories.Interfaces.Post
{
    public interface IPostRepository : IBaseRepository<Entities.Post>
    {
        public Task<IEnumerable<Entities.Post>> GetByUserIdAsync(Guid userId);
        public Task<IEnumerable<Entities.Post>> GetRecentAsync(int count);
        public Task<Entities.Post?> GetWithCommentsAsync(Guid id);
        public Task<Entities.Post?> GetWithCommentsAndReactionsAsync(Guid id);
        public Task<bool> ExistsAsync(Guid id);
        public Task<int> GetCountByUserAsync(Guid userId);
    }
}