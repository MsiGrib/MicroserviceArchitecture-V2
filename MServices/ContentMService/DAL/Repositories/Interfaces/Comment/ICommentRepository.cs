namespace DAL.Repositories.Interfaces.Comment
{
    public interface ICommentRepository : IBaseRepository<Entities.Comment>
    {
        public Task<IEnumerable<Entities.Comment>> GetByPostIdAsync(Guid postId);
        public Task<IEnumerable<Entities.Comment>> GetByUserIdAsync(Guid userId);
        public Task<int> GetCountByPostAsync(Guid postId);
        public Task<bool> UserOwnsCommentAsync(Guid commentId, Guid userId);
    }
}