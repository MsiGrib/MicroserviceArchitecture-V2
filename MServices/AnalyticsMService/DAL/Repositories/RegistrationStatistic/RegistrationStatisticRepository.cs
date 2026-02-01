using DAL.Repositories.Interfaces.RegistrationStatistic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.RegistrationStatistic
{
    public class RegistrationStatisticRepository : BaseRepository<Entities.RegistrationStatistic>, IRegistrationStatisticRepository
    {
        public RegistrationStatisticRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Entities.RegistrationStatistic>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.UTC)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entities.RegistrationStatistic>> GetByTimeRangeAsync(DateTime startUtc, DateTime endUtc)
        {
            return await _dbSet
                .Where(r => r.UTC >= startUtc && r.UTC <= endUtc)
                .OrderByDescending(r => r.UTC)
                .ToListAsync();
        }

        public async Task<int> GetCountBySourceTypeAsync(int sourceTypeId)
        {
            return await _dbSet
                .CountAsync(r => r.SourceTypeId == sourceTypeId);
        }

        public async Task<int> GetCountByStatusTypeAsync(int statusTypeId)
        {
            return await _dbSet
                .CountAsync(r => r.StatusTypeId == statusTypeId);
        }

        public async Task<Dictionary<int, int>> GetSourceTypeDistributionAsync(DateTime? startUtc = null, DateTime? endUtc = null)
        {
            var query = _dbSet.AsQueryable();

            if (startUtc.HasValue)
                query = query.Where(r => r.UTC >= startUtc.Value);

            if (endUtc.HasValue)
                query = query.Where(r => r.UTC <= endUtc.Value);

            return await query
                .GroupBy(r => r.SourceTypeId)
                .Select(g => new { SourceTypeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SourceTypeId, x => x.Count);
        }
    }
}