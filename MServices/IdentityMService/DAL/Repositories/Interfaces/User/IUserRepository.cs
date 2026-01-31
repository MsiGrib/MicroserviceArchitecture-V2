namespace DAL.Repositories.Interfaces.User
{
    public interface IUserRepository : IBaseRepository<Entities.User>
    {
        public Task<Entities.User?> GetByEmailAsync(string email);
        public Task<Entities.User?> GetByUsernameAsync(string username);
        public new Task<Entities.User?> GetByIdAsync(Guid id);
        public Task<List<Entities.User>?> GetByIdsAsync(List<Guid> ids);
        public Task<bool> ExistsByEmailAsync(string email);
        public Task<bool> ExistsByUsernameAsync(string username);
    }
}