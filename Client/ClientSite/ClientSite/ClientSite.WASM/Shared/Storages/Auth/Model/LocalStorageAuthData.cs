namespace ClientSite.WASM.Shared.Storages.Auth.Model
{
    public record LocalStorageAuthData
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public DateTime? TokenExpiry { get; set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken) && TokenExpiry > DateTime.UtcNow;
        public bool IsTokenExpired => TokenExpiry <= DateTime.UtcNow;
    }
}