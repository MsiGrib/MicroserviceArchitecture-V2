using BLL.DTOs.Comment.DTO;
using BLL.DTOs.Post.DTO;
using BLL.DTOs.Post.Requests;
using BLL.DTOs.Reaction.DTO;
using BLL.Services.Interfaces.Post;
using DAL.Entities;
using DAL.Repositories.Interfaces.Post;

namespace BLL.Services.Post
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Guid> CreatePostAsync(CreatePostRequest request, Guid userId)
        {
            var post = new DAL.Entities.Post
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _postRepository.AddAsync(post);
            await _postRepository.SaveChangesAsync();

            return post.Id;
        }

        public async Task<PostDTO?> GetPostByIdAsync(Guid id)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null) return null;

            return new PostDTO()
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
                Comments = post.Comments.Select(x => new CommentDTO
                {
                    Id = x.Id,
                    Text = x.Text,
                    UserId = x.UserId,
                    PostId = x.PostId,
                }).ToList(),
                Reactions = post.Reactions.Select(x => new ReactionDTO
                { 
                    Id = x.Id,
                    Type = x.Type,
                    UserId = x.UserId,
                    PostId = x.PostId,
                }).ToList(),
            };
        }

        public async Task<IEnumerable<PostDTO>?> GetAllPostsAsync()
        {
            var posts = await _postRepository.GetAllAsync();

            if (posts == null || posts.Count() == 0) return null;

            return posts.Select(x => new PostDTO
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                UserId = x.UserId,
                Comments = x.Comments.Select(y => new CommentDTO
                {
                    Id = y.Id,
                    Text = y.Text,
                    UserId = y.UserId,
                    PostId = y.PostId,
                }).ToList(),
                Reactions = x.Reactions.Select(y => new ReactionDTO
                {
                    Id = y.Id,
                    Type = y.Type,
                    UserId = y.UserId,
                    PostId = y.PostId,
                }).ToList(),
            });
        }

        public async Task<PostDTO> UpdatePostAsync(UpdatePostRequest request, Guid postId, Guid userId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new InvalidOperationException("Post not found");

            if (post.UserId != userId)
                throw new UnauthorizedAccessException("You are not the owner of this post");

            var updatedPost = post with
            {
                Title = request.Title,
                Content = request.Content,
                UpdatedAt = DateTime.UtcNow
            };

            _postRepository.Update(updatedPost);
            await _postRepository.SaveChangesAsync();

            return new PostDTO()
            {
                Id = updatedPost.Id,
                Title = updatedPost.Title,
                Content = updatedPost.Content,
                CreatedAt = updatedPost.CreatedAt,
                UserId = updatedPost.UserId,
                Comments = updatedPost.Comments.Select(x => new CommentDTO
                {
                    Id = x.Id,
                    Text = x.Text,
                    UserId = x.UserId,
                    PostId = x.PostId,
                }).ToList(),
                Reactions = updatedPost.Reactions.Select(x => new ReactionDTO
                {
                    Id = x.Id,
                    Type = x.Type,
                    UserId = x.UserId,
                    PostId = x.PostId,
                }).ToList(),
            };
        }

        public async Task<bool> DeletePostAsync(Guid postId, Guid userId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null || post.UserId != userId)
                return false;

            var deletedPost = post with { DeletedAt = DateTime.UtcNow };
            _postRepository.Update(deletedPost);

            return await _postRepository.SaveChangesAsync();
        }
    }
}