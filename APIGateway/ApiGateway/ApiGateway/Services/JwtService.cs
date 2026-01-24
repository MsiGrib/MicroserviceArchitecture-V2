using ApiGateway.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace ApiGateway.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateHeaderSignature(string userId, IEnumerable<string> roles)
        {
            var secret = _configuration["JwtSettings:Secret"];
            var data = $"{userId}:{string.Join(",", roles.OrderBy(r => r))}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

            return Convert.ToBase64String(hash);
        }

        public bool ValidateHeaderSignature(string userId, IEnumerable<string> roles, string signature)
        {
            var expected = CreateHeaderSignature(userId, roles);
            return expected == signature;
        }
    }
}