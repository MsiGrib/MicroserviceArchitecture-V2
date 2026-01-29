using BLL.DTOs.Reaction.Requests;
using BLL.Services.Interfaces.Reaction;
using Microsoft.AspNetCore.Mvc;

namespace ContentMService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReactionController : BaseController
    {
        private readonly IReactionService _reactionService;
        private readonly ILogger<ReactionController> _logger;

        public ReactionController(IReactionService reactionService, ILogger<ReactionController> logger)
        {
            _reactionService = reactionService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateReaction([FromBody] AddReactionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!IsUserAuthenticated())
                    return Unauthorized("User not authenticated");

                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                    return Unauthorized("User not authenticated");

                var reaction = await _reactionService.AddOrUpdateReactionAsync(request, userId);

                return Ok(reaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating reaction");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("post/{postId}")]
        public async Task<IActionResult> RemoveReaction([FromRoute] Guid postId)
        {
            try
            {
                if (!IsUserAuthenticated())
                    return Unauthorized("User not authenticated");

                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                    return Unauthorized("User not authenticated");

                var result = await _reactionService.RemoveReactionAsync(postId, userId);
                if (!result)
                    return NotFound("Reaction not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing reaction from post {PostId}", postId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetReactionsByPost(Guid postId)
        {
            try
            {
                var reactions = await _reactionService.GetReactionsByPostIdAsync(postId);
                return Ok(reactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reactions for post {PostId}", postId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}