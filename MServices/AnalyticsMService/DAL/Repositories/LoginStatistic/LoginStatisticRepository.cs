using DAL.Repositories.Interfaces.LoginStatistic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.LoginStatistic
{
    public class LoginStatisticRepository : BaseRepository<Entities.LoginStatistic>, ILoginStatisticRepository
    {
        public LoginStatisticRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Entities.LoginStatistic>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.UTC)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entities.LoginStatistic>> GetByTimeRangeAsync(DateTime startUtc, DateTime endUtc)
        {
            return await _dbSet
                .Where(l => l.UTC >= startUtc && l.UTC <= endUtc)
                .OrderByDescending(l => l.UTC)
                .ToListAsync();
        }

        public async Task<Entities.LoginStatistic?> GetLastLoginByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.UTC)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetLoginCountByUserAsync(Guid userId, DateTime? startUtc = null, DateTime? endUtc = null)
        {
            var query = _dbSet.Where(l => l.UserId == userId);

            if (startUtc.HasValue)
                query = query.Where(l => l.UTC >= startUtc.Value);

            if (endUtc.HasValue)
                query = query.Where(l => l.UTC <= endUtc.Value);

            return await query.CountAsync();
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

        public async Task<Dictionary<int, int>> GetSourceTypeDistributionAsync(DateTime? startUtc = null, DateTime? endUtc = null)
        {
            var query = _dbSet.AsQueryable();

            if (startUtc.HasValue)
                query = query.Where(l => l.UTC >= startUtc.Value);

            if (endUtc.HasValue)
                query = query.Where(l => l.UTC <= endUtc.Value);

            return await query
                .GroupBy(l => l.SourceTypeId)
                .Select(g => new { SourceTypeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SourceTypeId, x => x.Count);
        }

        public async Task<Dictionary<int, int>> GetStatusTypeDistributionAsync(DateTime? startUtc = null, DateTime? endUtc = null)
        {
            var query = _dbSet.AsQueryable();

            if (startUtc.HasValue)
                query = query.Where(l => l.UTC >= startUtc.Value);

            if (endUtc.HasValue)
                query = query.Where(l => l.UTC <= endUtc.Value);

            return await query
                .GroupBy(l => l.StatusTypeId)
                .Select(g => new { StatusTypeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.StatusTypeId, x => x.Count);
        }
    }
}