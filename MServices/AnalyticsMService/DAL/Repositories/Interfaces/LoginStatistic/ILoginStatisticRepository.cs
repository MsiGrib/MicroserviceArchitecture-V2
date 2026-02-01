namespace DAL.Repositories.Interfaces.LoginStatistic
{
    public interface ILoginStatisticRepository : IBaseRepository<Entities.LoginStatistic>
    {
        public Task<IEnumerable<Entities.LoginStatistic>> GetByUserIdAsync(Guid userId);
        public Task<IEnumerable<Entities.LoginStatistic>> GetByTimeRangeAsync(DateTime startUtc, DateTime endUtc);
        public Task<Entities.LoginStatistic?> GetLastLoginByUserAsync(Guid userId);
        public Task<int> GetLoginCountByUserAsync(Guid userId, DateTime? startUtc = null, DateTime? endUtc = null);
        public Task<int> GetCountBySourceTypeAsync(int sourceTypeId);
        public Task<int> GetCountByStatusTypeAsync(int statusTypeId);
        public Task<Dictionary<int, int>> GetSourceTypeDistributionAsync(DateTime? startUtc = null, DateTime? endUtc = null);
        public Task<Dictionary<int, int>> GetStatusTypeDistributionAsync(DateTime? startUtc = null, DateTime? endUtc = null);
    }
}