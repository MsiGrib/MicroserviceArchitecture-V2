using BLL.DTOs.Comment.DTO;
using BLL.DTOs.Comment.Requests;
using BLL.Integrations.IdentityMService.DTOs;
using BLL.Integrations.IdentityMService.Interfaces;
using BLL.Services.Interfaces.Comment;
using DAL.Repositories.Interfaces.Comment;
using DAL.Repositories.Interfaces.Post;

namespace BLL.Services.Comment
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IIdentityServiceIntegration _identityService;

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, IIdentityServiceIntegration identityService)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _identityService = identityService;
        }

        public async Task<Guid> CreateCommentAsync(CreateCommentRequest request, Guid userId)
        {
            var postExists = await _postRepository.ExistsAsync(request.PostId);
            if (!postExists)
                throw new InvalidOperationException("Post not found");

            var comment = new DAL.Entities.Comment
            {
                Id = Guid.NewGuid(),
                PostId = request.PostId,
                UserId = userId,
                Text = request.Text
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();

            return comment.Id;
        }

        public async Task<IEnumerable<CommentDTO>?> GetCommentsByPostIdAsync(Guid postId)
        {
            var comments = await _commentRepository.GetByPostIdAsync(postId);

            var usersInfo = await _identityService.GetUsersSmallInfoAsync(comments.Select(x => x.UserId).ToList());

            if (usersInfo == null || usersInfo.Count == 0) return null;

            return comments.Select(x => new CommentDTO
            {
                Id = x.Id,
                Text = x.Text,
                UserInfo = new UserSmallInfoDTO
                {
                    Id = usersInfo.First(y => x.UserId == y.Id).Id,
                    Email = usersInfo.First(y => x.UserId == y.Id).Email,
                    Username = usersInfo.First(y => x.UserId == y.Id).Username,
                },
                PostId = x.PostId,
            });
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.UserId != userId)
                return false;

            _commentRepository.Remove(comment);
            return await _commentRepository.SaveChangesAsync();
        }
    }
}