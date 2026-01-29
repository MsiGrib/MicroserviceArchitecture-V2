using System.Security.Claims;

namespace ContentMService.Middleware
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
            var path = context.Request.Path.ToString().ToLower();
            var method = context.Request.Method.ToUpper();

            _logger.LogDebug("GatewayAuthMiddleware: Path={Path}, Method={Method}", path, method);

            bool isPublicEndpoint = IsPublicEndpoint(path, method);

            _logger.LogDebug("IsPublicEndpoint: {IsPublic}", isPublicEndpoint);

            if (isPublicEndpoint)
            {
                _logger.LogDebug("Public endpoint, skipping auth check");
                await _next(context);
                return;
            }

            var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            var signature = context.Request.Headers["X-Auth-Signature"].FirstOrDefault();
            var userEmail = context.Request.Headers["X-User-Email"].FirstOrDefault();
            var rolesHeader = context.Request.Headers["X-User-Roles"].FirstOrDefault();

            _logger.LogDebug("Headers - UserId: {UserId}, HasSignature: {HasSignature}, Email: {Email}, Roles: {Roles}",
                userId, !string.IsNullOrEmpty(signature), userEmail, rolesHeader);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning("Missing authentication headers for protected endpoint");
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "Missing authentication headers"
                });
                return;
            }

            var roles = string.IsNullOrEmpty(rolesHeader)
                ? Array.Empty<string>()
                : rolesHeader.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var secret = _configuration["Jwt:Key"]
                ?? _configuration["JwtSettings:Secret"]
                ?? throw new InvalidOperationException("JWT secret is not configured");

            if (!ValidateHeaderSignature(userId, roles, signature, secret))
            {
                _logger.LogWarning("Invalid signature for user {UserId}", userId);
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "Invalid authentication signature"
                });
                return;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            if (!string.IsNullOrEmpty(userEmail))
                claims.Add(new Claim(ClaimTypes.Email, userEmail));

            foreach (var role in roles.Where(r => !string.IsNullOrEmpty(r)))
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, "Gateway");
            context.User = new ClaimsPrincipal(identity);

            _logger.LogDebug("User authenticated: {UserId}, Roles: {RolesCount}", userId, roles.Length);

            await _next(context);
        }

        private bool IsPublicEndpoint(string path, string method)
        {
            var publicPatterns = new List<(string Method, string PathPattern)>
            {
                ("GET", "/api/posts"),
                ("GET", @"/api/posts/[0-9a-fA-F-]{36}"), // GET /api/posts/{id}
                ("GET", @"/api/comments/post/[0-9a-fA-F-]{36}"), // GET /api/comments/post/{postId}
                ("GET", @"/api/reactions/post/[0-9a-fA-F-]{36}"), // GET /api/reactions/post/{postId}
            };

            foreach (var pattern in publicPatterns)
            {
                if (method == pattern.Method && MatchesPattern(path, pattern.PathPattern))
                    return true;
            }

            return false;
        }

        private bool MatchesPattern(string path, string pattern)
        {
            if (pattern.Contains('['))
            {
                var regexPattern = "^" + pattern.Replace("[0-9a-fA-F-]{36}", "[0-9a-fA-F-]{8}-[0-9a-fA-F-]{4}-[0-9a-fA-F-]{4}-[0-9a-fA-F-]{4}-[0-9a-fA-F-]{12}") + "$";
                return System.Text.RegularExpressions.Regex.IsMatch(path, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return path.Equals(pattern, StringComparison.OrdinalIgnoreCase);
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
                var sortedRoles = roles.OrderBy(r => r).ToList();
                var data = $"{userId}:{string.Join(",", sortedRoles)}";

                using var hmac = new System.Security.Cryptography.HMACSHA256(
                    System.Text.Encoding.UTF8.GetBytes(secret));
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
                var expectedSignature = Convert.ToBase64String(hash);

                var isValid = expectedSignature == signature;

                if (!isValid)
                {
                    _logger.LogDebug("Signature mismatch. Expected: {Expected}, Actual: {Actual}",
                        expectedSignature, signature);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating signature");
                return false;
            }
        }
    }
}