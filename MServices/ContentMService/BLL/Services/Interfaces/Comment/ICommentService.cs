using BLL.DTOs.Comment.DTO;
using BLL.DTOs.Comment.Requests;

namespace BLL.Services.Interfaces.Comment
{
    public interface ICommentService
    {
        public Task<Guid> CreateCommentAsync(CreateCommentRequest request, Guid userId);
        public Task<IEnumerable<CommentDTO>?> GetCommentsByPostIdAsync(Guid postId);
        public Task<bool> DeleteCommentAsync(Guid commentId, Guid userId);
    }
}