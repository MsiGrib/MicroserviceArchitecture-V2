namespace ApiGateway.Services.Interfaces
{
    public interface IJwtService
    {
        public string CreateHeaderSignature(string userId, IEnumerable<string> roles);
        public bool ValidateHeaderSignature(string userId, IEnumerable<string> roles, string signature);
    }
}