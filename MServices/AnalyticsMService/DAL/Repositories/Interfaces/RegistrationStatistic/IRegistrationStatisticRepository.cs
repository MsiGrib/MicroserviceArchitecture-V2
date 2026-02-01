namespace DAL.Repositories.Interfaces.RegistrationStatistic
{
    public interface IRegistrationStatisticRepository : IBaseRepository<Entities.RegistrationStatistic>
    {
        public Task<IEnumerable<Entities.RegistrationStatistic>> GetByUserIdAsync(Guid userId);
        public Task<IEnumerable<Entities.RegistrationStatistic>> GetByTimeRangeAsync(DateTime startUtc, DateTime endUtc);
        public Task<int> GetCountBySourceTypeAsync(int sourceTypeId);
        public Task<int> GetCountByStatusTypeAsync(int statusTypeId);
        public Task<Dictionary<int, int>> GetSourceTypeDistributionAsync(DateTime? startUtc = null, DateTime? endUtc = null);
    }
}