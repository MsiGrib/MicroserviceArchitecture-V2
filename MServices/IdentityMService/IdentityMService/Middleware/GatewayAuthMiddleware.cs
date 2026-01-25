using System.Security.Claims;

namespace IdentityMService.Middleware
{
    public class GatewayAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public GatewayAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine($"GatewayAuthMiddleware: Path = {context.Request.Path}");

            bool isPublicEndpoint =
                context.Request.Path.StartsWithSegments("auth/login") ||
                context.Request.Path.StartsWithSegments("auth/register") ||
                context.Request.Path.StartsWithSegments("auth/refresh");

            Console.WriteLine($"IsPublicEndpoint: {isPublicEndpoint}");

            if (isPublicEndpoint)
            {
                Console.WriteLine("Skipping auth check for public endpoint");
                await _next(context);
                return;
            }

            var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            var signature = context.Request.Headers["X-Auth-Signature"].FirstOrDefault();

            Console.WriteLine($"X-User-Id: {userId}");
            Console.WriteLine($"X-Auth-Signature: {signature}");

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(signature))
            {
                var rolesHeader = context.Request.Headers["X-User-Roles"].ToString();
                var roles = string.IsNullOrEmpty(rolesHeader)
                    ? Array.Empty<string>()
                    : rolesHeader.Split(',', StringSplitOptions.RemoveEmptyEntries);

                var secret = _configuration["Jwt:Key"] ?? _configuration["JwtSettings:Secret"];

                Console.WriteLine($"Secret configured: {!string.IsNullOrEmpty(secret)}");
                Console.WriteLine($"Roles: {string.Join(", ", roles)}");

                if (!ValidateHeaderSignature(userId, roles, signature, secret))
                {
                    Console.WriteLine("Invalid signature");
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
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
                Console.WriteLine($"User authenticated: {context.User.Identity?.IsAuthenticated}");
            }
            else if (context.Request.Headers.ContainsKey("Authorization"))
            {
                Console.WriteLine("Using Authorization header");
                await _next(context);
                return;
            }
            else
            {
                bool isProtectedEndpoint =
                    context.Request.Path.StartsWithSegments("auth/logout") ||
                    context.Request.Path.StartsWithSegments("auth/change-password");

                Console.WriteLine($"IsProtectedEndpoint: {isProtectedEndpoint}");

                if (isProtectedEndpoint)
                {
                    Console.WriteLine("Unauthorized access to protected endpoint");
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Unauthorized",
                        message = "Missing authentication headers"
                    });
                    return;
                }
            }

            await _next(context);
        }

        private bool ValidateHeaderSignature(string userId, IEnumerable<string> roles, string signature, string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                Console.WriteLine("Validation failed: Secret is empty");
                return false;
            }

            try
            {
                var data = $"{userId}:{string.Join(",", roles.OrderBy(r => r))}";

                using var hmac = new System.Security.Cryptography.HMACSHA256(
                    System.Text.Encoding.UTF8.GetBytes(secret));
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
                var expectedSignature = Convert.ToBase64String(hash);

                bool isValid = expectedSignature == signature;
                Console.WriteLine($"Signature validation: {isValid}");
                Console.WriteLine($"Expected: {expectedSignature}");
                Console.WriteLine($"Received: {signature}");

                return isValid;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating signature: {ex.Message}");
                return false;
            }
        }
    }
}