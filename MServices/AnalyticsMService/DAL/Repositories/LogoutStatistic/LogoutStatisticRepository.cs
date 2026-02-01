using DAL.Repositories.Interfaces.LogoutStatistic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.LogoutStatistic
{
    public class LogoutStatisticRepository : BaseRepository<Entities.LogoutStatistic>, ILogoutStatisticRepository
    {
        public LogoutStatisticRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Entities.LogoutStatistic>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.UTC)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entities.LogoutStatistic>> GetByTimeRangeAsync(DateTime startUtc, DateTime endUtc)
        {
            return await _dbSet
                .Where(l => l.UTC >= startUtc && l.UTC <= endUtc)
                .OrderByDescending(l => l.UTC)
                .ToListAsync();
        }

        public async Task<Entities.LogoutStatistic?> GetLastLogoutByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.UTC)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetCountBySourceTypeAsync(int sourceTypeId)
        {
            return await _dbSet
                .CountAsync(l => l.SourceTypeId == sourceTypeId);
        }

        public async Task<int> GetCountByStatusTypeAsync(int statusTypeId)
        {
            return await _dbSet
                .CountAsync(l => l.StatusTypeId == statusTypeId);
        }
    }
}