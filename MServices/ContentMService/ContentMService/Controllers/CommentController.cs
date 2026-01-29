using BLL.DTOs.Comment.Requests;
using BLL.Services.Interfaces.Comment;
using Microsoft.AspNetCore.Mvc;

namespace ContentMService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : BaseController
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentService commentService, ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsByPost([FromRoute] Guid postId)
        {
            try
            {
                var comments = await _commentService.GetCommentsByPostIdAsync(postId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for post {PostId}", postId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
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

                var commentId = await _commentService.CreateCommentAsync(request, userId);

                return Ok(commentId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid id)
        {
            try
            {
                if (!IsUserAuthenticated())
                    return Unauthorized("User not authenticated");

                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                    return Unauthorized("User not authenticated");

                var result = await _commentService.DeleteCommentAsync(id, userId);
                if (!result)
                    return NotFound("Comment not found or you don't have permission to delete it");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment {CommentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}