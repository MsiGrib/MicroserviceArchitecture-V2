namespace DAL.Repositories.Interfaces.LogoutStatistic
{
    public interface ILogoutStatisticRepository : IBaseRepository<Entities.LogoutStatistic>
    {
        public Task<IEnumerable<Entities.LogoutStatistic>> GetByUserIdAsync(Guid userId);
        public Task<IEnumerable<Entities.LogoutStatistic>> GetByTimeRangeAsync(DateTime startUtc, DateTime endUtc);
        public Task<Entities.LogoutStatistic?> GetLastLogoutByUserAsync(Guid userId);
        public Task<int> GetCountBySourceTypeAsync(int sourceTypeId);
        public Task<int> GetCountByStatusTypeAsync(int statusTypeId);
    }
}