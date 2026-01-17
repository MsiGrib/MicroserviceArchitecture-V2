namespace DAL.Repositories.Interfaces.User
{
    public interface IUserRepository : IBaseRepository<Entities.User>
    {
        public Task<Entities.User?> GetByEmailAsync(string email);
        public Task<Entities.User?> GetByUsernameAsync(string username);
        public new Task<Entities.User?> GetByIdAsync(Guid id);
        public Task<bool> ExistsByEmailAsync(string email);
        public Task<bool> ExistsByUsernameAsync(string username);
    }
}