using BLL.DTOs.Comment.DTO;
using BLL.DTOs.Comment.Requests;
using BLL.Services.Interfaces.Comment;
using DAL.Entities;
using DAL.Repositories.Interfaces.Comment;
using DAL.Repositories.Interfaces.Post;

namespace BLL.Services.Comment
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
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

        public async Task<IEnumerable<CommentDTO>> GetCommentsByPostIdAsync(Guid postId)
        {
            var comments = await _commentRepository.GetByPostIdAsync(postId);

            return comments.Select(x => new CommentDTO
            {
                Id = x.Id,
                Text = x.Text,
                UserId = x.UserId,
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