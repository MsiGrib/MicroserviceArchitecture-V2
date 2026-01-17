namespace DAL.Repositories.Interfaces
{
    public interface IBaseRepository<T>
        where T : class
    {
        public Task<T?> GetByIdAsync(Guid id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task AddAsync(T entity);
        public Task AddRangeAsync(IEnumerable<T> entities);
        public void Update(T entity);
        public void UpdateRange(IEnumerable<T> entities);
        public void Remove(T entity);
        public void RemoveRange(IEnumerable<T> entities);
        public Task<bool> SaveChangesAsync();
    }
}