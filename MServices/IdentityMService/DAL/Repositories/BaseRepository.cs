using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public virtual async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
            => await _dbSet.AddRangeAsync(entities);

        public virtual void Update(T entity)
            => _dbSet.Update(entity);

        public virtual void UpdateRange(IEnumerable<T> entities)
            => _dbSet.UpdateRange(entities);

        public virtual void Remove(T entity)
            => _dbSet.Remove(entity);

        public virtual void RemoveRange(IEnumerable<T> entities)
            => _dbSet.RemoveRange(entities);

        public virtual async Task<bool> SaveChangesAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}