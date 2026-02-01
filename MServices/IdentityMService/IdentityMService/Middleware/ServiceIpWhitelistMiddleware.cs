using Microsoft.Extensions.Primitives;

namespace IdentityMService.Middleware
{
    public class ServiceIpWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceIpWhitelistMiddleware> _logger;
        private readonly HashSet<string> _allowedIps;
        private readonly List<string> _endpoints = new() 
        {
            "/small-info", "/small-info/batch",
        };

        public ServiceIpWhitelistMiddleware(RequestDelegate next, IConfiguration configuration,
            ILogger<ServiceIpWhitelistMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;

            var allowedIps = _configuration.GetSection("Services:ContentService:AllowedIps")
                .Get<string[]>() ?? Array.Empty<string>();
            _allowedIps = new HashSet<string>(allowedIps);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";

            if (_endpoints.Contains(path))
            {
                var apiKey = context.Request.Headers["X-Service-Api-Key"].FirstOrDefault();

                if (!string.IsNullOrEmpty(apiKey))
                {
                    var clientIp = GetClientIp(context);

                    if (!_allowedIps.Contains(clientIp))
                    {
                        _logger.LogWarning("Blocked request from unauthorized IP: {Ip}", clientIp);
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Forbidden",
                            message = "IP address not authorized"
                        });
                        return;
                    }
                }
            }

            await _next(context);
        }

        private string GetClientIp(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues forwardedFor))
            {
                return forwardedFor.FirstOrDefault()?.Split(',')[0]?.Trim()
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}