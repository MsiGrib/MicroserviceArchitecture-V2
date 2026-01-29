using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ContentMService.Middleware
{
    public class GatewayAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public GatewayAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
                UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                var ticketNew = new AuthenticationTicket(
                    Context.User,
                    new AuthenticationProperties(),
                    Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticketNew));
            }

            var userId = Context.Request.Headers["X-User-Id"].FirstOrDefault();
            var signature = Context.Request.Headers["X-Auth-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(signature))
                return Task.FromResult(AuthenticateResult.NoResult());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var rolesHeader = Context.Request.Headers["X-User-Roles"].FirstOrDefault();
            if (!string.IsNullOrEmpty(rolesHeader))
            {
                var roles = rolesHeader.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}