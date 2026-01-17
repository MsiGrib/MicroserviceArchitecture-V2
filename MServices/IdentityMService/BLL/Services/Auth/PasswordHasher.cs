using BLL.Services.Interfaces.Auth;
using System.Security.Cryptography;

namespace BLL.Services.Auth
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        public string HashPassword(string password)
        {
            using var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256);

            var salt = algorithm.Salt;
            var key = algorithm.GetBytes(KeySize);

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(key)}";
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var parts = hashedPassword.Split(':');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedKey = Convert.FromBase64String(parts[1]);

            using var algorithm = new Rfc2898DeriveBytes(providedPassword, salt, Iterations, HashAlgorithmName.SHA256);

            var providedKey = algorithm.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(storedKey, providedKey);
        }
    }
}