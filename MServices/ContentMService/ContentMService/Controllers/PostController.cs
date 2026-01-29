using BLL.DTOs.Post.Requests;
using BLL.Services.Interfaces.Post;
using Microsoft.AspNetCore.Mvc;

namespace ContentMService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : BaseController
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all posts");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost([FromRoute] Guid id)
        {
            try
            {
                var post = await _postService.GetPostByIdAsync(id);
                if (post == null)
                    return NotFound($"Post with id {id} not found");

                return Ok(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting post {PostId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
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

                var postId = await _postService.CreatePostAsync(request, userId);

                return Ok(postId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost([FromRoute] Guid id, [FromBody] UpdatePostRequest request)
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

                var post = await _postService.UpdatePostAsync(request, id, userId);

                return Ok(post);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("You are not the owner of this post");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post {PostId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            try
            {
                if (!IsUserAuthenticated())
                    return Unauthorized("User not authenticated");

                var userId = GetUserIdFromClaims();
                if (userId == Guid.Empty)
                    return Unauthorized("User not authenticated");

                var result = await _postService.DeletePostAsync(id, userId);
                if (!result)
                    return NotFound("Post not found or you don't have permission to delete it");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post {PostId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}