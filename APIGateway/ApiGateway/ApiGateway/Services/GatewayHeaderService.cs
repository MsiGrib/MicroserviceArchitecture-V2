using ApiGateway.Services.Interfaces;
using System.Security.Claims;

namespace ApiGateway.Services
{
    public class GatewayHeaderService : IGatewayHeaderService
    {
        private readonly IJwtService _jwtService;

        private static readonly Dictionary<string, string[]> PublicEndpoints = new()
        {
            ["auth"] = new[] { "POST /login", "POST /register", "POST /refresh" },
            ["post"] = new[] { "GET /" },
            ["comment"] = new[] { "GET /post/{postId}" },
        };
        private static readonly List<string> IdentityProtectedEndpoints = new()
        {
            "/api/auth/logout",
            "/api/auth/change-password",
            "/api/user/me"
        };

        public GatewayHeaderService(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task TransformRequestAsync(HttpContext context, HttpRequestMessage proxyRequest)
        {
            var path = context.Request.Path.ToString();
            var method = context.Request.Method;


            if (IsPublicEndpoint(context))
            {
                HandlePublicEndpoint(context, proxyRequest);
            }
            else if (IsIdentityProtectedEndpoint(path, method))
            {
                await HandleIdentityProtectedEndpoint(context, proxyRequest);
            }
            else if (path.StartsWith("/api/auth/", StringComparison.OrdinalIgnoreCase) ||
                     path.StartsWith("/api/user/", StringComparison.OrdinalIgnoreCase))
            {
                await HandleIdentityEndpoint(context, proxyRequest);
            }
            else
            {
                await HandleContentEndpoint(context, proxyRequest);
            }
        }

        private bool IsIdentityProtectedEndpoint(string path, string method)
        {
            var endpoint = $"{method} {path}";
            return IdentityProtectedEndpoints.Any(p => endpoint.Contains(p, StringComparison.OrdinalIgnoreCase));
        }

        private async Task HandleIdentityProtectedEndpoint(HttpContext context, HttpRequestMessage proxyRequest)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                var isValid = await _jwtService.ValidateTokenAsync(token);

                if (isValid)
                {
                    var claimsPrincipal = await _jwtService.GetPrincipalFromTokenAsync(token);
                    if (claimsPrincipal != null)
                    {
                        await AddGatewayHeadersAsync(claimsPrincipal, proxyRequest);
                    }
                }
            }

            if (!string.IsNullOrEmpty(authHeader))
            {
                proxyRequest.Headers.TryAddWithoutValidation("Authorization", authHeader);
            }
        }

        private async Task HandleIdentityEndpoint(HttpContext context, HttpRequestMessage proxyRequest)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader))
            {
                proxyRequest.Headers.TryAddWithoutValidation("Authorization", authHeader);

                if (authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    var isValid = await _jwtService.ValidateTokenAsync(token);

                    if (isValid)
                    {
                        var claimsPrincipal = await _jwtService.GetPrincipalFromTokenAsync(token);
                        if (claimsPrincipal != null)
                        {
                            await AddGatewayHeadersAsync(claimsPrincipal, proxyRequest);
                        }
                    }
                }
            }
        }

        private async Task HandleContentEndpoint(HttpContext context, HttpRequestMessage proxyRequest)
        {
            proxyRequest.Headers.Remove("Authorization");

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var isValid = await _jwtService.ValidateTokenAsync(token);

                if (isValid)
                {
                    var claimsPrincipal = await _jwtService.GetPrincipalFromTokenAsync(token);
                    if (claimsPrincipal != null)
                        await AddGatewayHeadersAsync(claimsPrincipal, proxyRequest);
                }
            }
        }

        private void HandlePublicEndpoint(HttpContext context, HttpRequestMessage proxyRequest)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
                proxyRequest.Headers.TryAddWithoutValidation("Authorization", authHeader);
        }

        private async Task AddGatewayHeadersAsync(ClaimsPrincipal user, HttpRequestMessage proxyRequest)
        {
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