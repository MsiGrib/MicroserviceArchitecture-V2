using System.Security.Claims;

namespace IdentityMService.Middleware
{
    public class GatewayAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GatewayAuthMiddleware> _logger;

        public GatewayAuthMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<GatewayAuthMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogDebug($"GatewayAuthMiddleware: Path = {context.Request.Path}, Method = {context.Request.Method}");

            var apiKey = context.Request.Headers["X-Service-Api-Key"].FirstOrDefault();
            if (!string.IsNullOrEmpty(apiKey))
            {
                _logger.LogDebug("Request with API Key, checking service-to-service authentication");
                var validApiKey = _configuration["Services:ContentService:ApiKey"];

                if (apiKey == validApiKey)
                {
                    if (context.Request.Path.Value?.Contains("small-info") == true)
                    {
                        var allowedIps = _configuration
                            .GetSection("Services:ContentService:AllowedIps")
                            .Get<string[]>() ?? Array.Empty<string>();

                        var clientIp = GetClientIp(context);
                        if (!allowedIps.Contains(clientIp))
                        {
                            _logger.LogWarning("Blocked API Key request from unauthorized IP: {Ip}", clientIp);
                            context.Response.StatusCode = 403;
                            await context.Response.WriteAsJsonAsync(new { error = "IP not authorized" });
                            return;
                        }
                    }

                    _logger.LogDebug("API Key authentication successful");
                    await _next(context);
                    return;
                }
                else
                {
                    _logger.LogWarning("Invalid API Key");
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key" });
                    return;
                }
            }

            bool isPublicEndpoint = IsPublicEndpoint(context);

            _logger.LogDebug($"IsPublicEndpoint: {isPublicEndpoint}");

            if (isPublicEndpoint)
            {
                _logger.LogDebug("Skipping auth check for public endpoint");
                await _next(context);
                return;
            }

            var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            var signature = context.Request.Headers["X-Auth-Signature"].FirstOrDefault();

            _logger.LogDebug($"X-User-Id: {userId}");
            _logger.LogDebug($"X-Auth-Signature: {signature}");

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(signature))
            {
                var rolesHeader = context.Request.Headers["X-User-Roles"].ToString();
                var roles = string.IsNullOrEmpty(rolesHeader)
                    ? Array.Empty<string>()
                    : rolesHeader.Split(',', StringSplitOptions.RemoveEmptyEntries);

                var secret = _configuration["Jwt:Key"] ?? _configuration["JwtSettings:Secret"];

                _logger.LogDebug($"Roles: {string.Join(", ", roles)}");

                if (!ValidateHeaderSignature(userId, roles, signature, secret))
                {
                    _logger.LogWarning("Invalid signature from API Gateway");
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid signature" });
                    return;
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                };

                foreach (var role in roles.Where(r => !string.IsNullOrEmpty(r)))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var identity = new ClaimsIdentity(claims, "Gateway");
                context.User = new ClaimsPrincipal(identity);
                _logger.LogDebug($"User authenticated via Gateway: {context.User.Identity?.IsAuthenticated}");

                await _next(context);
                return;
            }
            else if (context.Request.Headers.ContainsKey("Authorization"))
            {
                _logger.LogDebug("Using Authorization header for direct call");
                await _next(context);
                return;
            }
            else
            {
                _logger.LogWarning("Unauthorized access to protected endpoint");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "Missing authentication"
                });
                return;
            }
        }

        private bool IsPublicEndpoint(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var method = context.Request.Method.ToUpper();

            return (path.Contains("/api/auth/login") && method == "POST") ||
                   (path.Contains("/api/auth/register") && method == "POST") ||
                   (path.Contains("/api/auth/refresh") && method == "POST");
        }

        private bool ValidateHeaderSignature(string userId, IEnumerable<string> roles, string signature, string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                _logger.LogError("JWT secret is not configured");
                return false;
            }

            try
            {
                var data = $"{userId}:{string.Join(",", roles.OrderBy(r => r))}";

                using var hmac = new System.Security.Cryptography.HMACSHA256(
                    System.Text.Encoding.UTF8.GetBytes(secret));
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
                var expectedSignature = Convert.ToBase64String(hash);

                return expectedSignature == signature;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating signature");
                return false;
            }
        }

        private string GetClientIp(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                return forwardedFor.FirstOrDefault()?.Split(',')[0]?.Trim()
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}