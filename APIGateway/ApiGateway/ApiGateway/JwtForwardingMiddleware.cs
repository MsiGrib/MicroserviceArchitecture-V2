using System.Security.Claims;

namespace ApiGateway
{
    public class JwtForwardingMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtForwardingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader))
                    context.Items["ForwardedAuthorization"] = authHeader;

                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

                if (!string.IsNullOrEmpty(userId))
                    context.Items["X-User-Id"] = userId;
            }

            await _next(context);
        }
    }
}