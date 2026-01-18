using BLL.Services.Interfaces.Auth;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Services.Auth
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int DegreeOfParallelism = 4;
        private const int MemorySize = 65536;   // 64 MB
        private const int Iterations = 4;
        private const int SaltSize = 16;    // 128 бит
        private const int HashSize = 32;    // 256 бит

        public string HashPassword(string password)
        {
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = DegreeOfParallelism,
                MemorySize = MemorySize,
                Iterations = Iterations
            };

            var hash = argon2.GetBytes(HashSize);

            return $"argon2id:{DegreeOfParallelism}:{MemorySize}:{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            try
            {
                var parts = hashedPassword.Split(':');

                if (parts.Length != 6 || parts[0] != "argon2id")
                    return false;

                if (!int.TryParse(parts[1], out var parallelism) ||
                    !int.TryParse(parts[2], out var memorySize) ||
                    !int.TryParse(parts[3], out var iterations))
                    return false;

                var salt = Convert.FromBase64String(parts[4]);
                var storedHash = Convert.FromBase64String(parts[5]);

                var argon2 = new Argon2id(Encoding.UTF8.GetBytes(providedPassword))
                {
                    Salt = salt,
                    DegreeOfParallelism = parallelism,
                    MemorySize = memorySize,
                    Iterations = iterations
                };

                var providedHash = argon2.GetBytes(storedHash.Length);

                return CryptographicOperations.FixedTimeEquals(storedHash, providedHash);
            }
            catch
            {
                return false;
            }
        }
    }
}