namespace BLL.Services.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}