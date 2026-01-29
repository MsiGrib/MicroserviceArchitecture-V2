using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContentMService.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetUserIdFromClaims()
        {
            var headerUserId = Request.Headers["X-User-Id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(headerUserId) && Guid.TryParse(headerUserId, out var userId))
                return userId;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userIdFromClaim))
                return userIdFromClaim;

            return Guid.Empty;
        }

        protected bool IsUserAuthenticated()
            => !string.IsNullOrEmpty(Request.Headers["X-User-Id"].FirstOrDefault()) ||
                   !string.IsNullOrEmpty(Request.Headers["X-Auth-Signature"].FirstOrDefault()) ||
                   User?.Identity?.IsAuthenticated == true;
    }
}