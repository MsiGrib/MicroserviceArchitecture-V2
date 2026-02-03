using BLL.Services.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _userService = userService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var result = await _userService.GetCurrentUser(userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("small-info/{userId}")]
        public async Task<IActionResult> GetSmallInfo([FromRoute] Guid userId)
        {
            try
            {
                var result = await _userService.GetUserSmallInfo(userId);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user small info for {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("small-info/batch")]
        public async Task<IActionResult> GetBatchSmallInfo([FromBody] List<Guid> userIds)
        {
            try
            {
                var result = await _userService.GetBatchUserSmallInfo(userIds);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}