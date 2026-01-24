using ApiGateway.Services.Interfaces;
using System.Security.Claims;

namespace ApiGateway.Services
{
    public class GatewayHeaderService : IGatewayHeaderService
    {
        private readonly IJwtService _jwtService;
        private static readonly Dictionary<string, string[]> PublicEndpoints = new()
        {
            ["auth"] = new[] { "POST /login", "POST /register", "POST /refresh", },
        };
        private static readonly List<string> IdentityServicePaths = new()
        {
            "/api/auth",
            "/api/user",
        };

        public GatewayHeaderService(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public bool IsIdentityServiceEndpoint(HttpContext context)
        {
            var path = context.Request.Path.ToString();

            return IdentityServicePaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }

        public async Task TransformRequestAsync(HttpContext context, HttpRequestMessage proxyRequest)
        {
            var user = context.User;

            if (IsPublicEndpoint(context))
            {
                HandlePublicAuthEndpoint(context, proxyRequest);
            }
            else if (IsIdentityServiceEndpoint(context))
            {
                HandleIdentityServiceEndpoint(context, proxyRequest);
            }
            else
            {
                HandleOtherServices(context, proxyRequest);
            }

            if (user?.Identity?.IsAuthenticated == true)
                await AddUserHeadersAsync(context, proxyRequest);

            await Task.CompletedTask;
        }

        private void HandlePublicAuthEndpoint(HttpContext context, HttpRequestMessage proxyRequest)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
                proxyRequest.Headers.TryAddWithoutValidation("Authorization", authHeader);
        }

        private void HandleIdentityServiceEndpoint(HttpContext context, HttpRequestMessage proxyRequest)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
                proxyRequest.Headers.TryAddWithoutValidation("Authorization", authHeader);
        }

        private void HandleOtherServices(HttpContext context, HttpRequestMessage proxyRequest)
        {
            proxyRequest.Headers.Remove("Authorization");
        }

        private async Task AddUserHeadersAsync(HttpContext context, HttpRequestMessage proxyRequest)
        {
            var user = context.User;

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);

            AddOrUpdateHeader(proxyRequest, "X-User-Id", userId);
            AddOrUpdateHeader(proxyRequest, "X-User-Email", email);

            if (roles.Any())
                AddOrUpdateHeader(proxyRequest, "X-User-Roles", string.Join(",", roles));

            var signature = _jwtService.CreateHeaderSignature(userId, roles);
            AddOrUpdateHeader(proxyRequest, "X-Auth-Signature", signature);
        }

        private void AddOrUpdateHeader(HttpRequestMessage request, string headerName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                request.Headers.Remove(headerName);
                request.Headers.Add(headerName, value);
            }
        }

        private bool IsPublicEndpoint(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;

            var controller = GetControllerName(path);
            if (string.IsNullOrEmpty(controller) || !PublicEndpoints.ContainsKey(controller))
                return false;

            var publicMethods = PublicEndpoints[controller];
            if (publicMethods.Length == 0)
                return false;

            foreach (var publicMethod in publicMethods)
            {
                if (IsMethodMatch(method, path, publicMethod))
                    return true;
            }

            return false;
        }

        private string GetControllerName(PathString path)
        {
            var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments?.Length >= 2 && segments[0].Equals("api", StringComparison.OrdinalIgnoreCase))
                return segments[1].ToLower();

            return string.Empty;
        }

        private bool IsMethodMatch(string httpMethod, PathString requestPath, string pattern)
        {
            var patternParts = pattern.Split(' ', 2);
            var patternMethod = patternParts[0];
            var patternPath = patternParts.Length > 1 ? patternParts[1] : null;

            if (!httpMethod.Equals(patternMethod, StringComparison.OrdinalIgnoreCase))
                return false;

            if (patternPath == null)
                return true;

            var controllerName = GetControllerName(requestPath);
            var fullPath = requestPath.Value ?? string.Empty;
            var relativePath = fullPath.Replace($"/api/{controllerName}", "");

            var normalizedPatternPath = patternPath.Trim('/');
            var normalizedRelativePath = relativePath.Trim('/');

            return normalizedRelativePath.Equals(normalizedPatternPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}