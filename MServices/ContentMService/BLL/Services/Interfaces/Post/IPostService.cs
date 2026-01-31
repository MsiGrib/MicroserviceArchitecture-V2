using BLL.DTOs.Post.DTO;
using BLL.DTOs.Post.Requests;

namespace BLL.Services.Interfaces.Post
{
    public interface IPostService
    {
        public Task<Guid> CreatePostAsync(CreatePostRequest request, Guid userId);
        public Task<PostDTO?> GetPostByIdAsync(Guid id);
        public Task<IEnumerable<PostDTO>?> GetAllPostsAsync();
        public Task<PostDTO?> UpdatePostAsync(UpdatePostRequest request, Guid postId, Guid userId);
        public Task<bool> DeletePostAsync(Guid postId, Guid userId);
    }
}