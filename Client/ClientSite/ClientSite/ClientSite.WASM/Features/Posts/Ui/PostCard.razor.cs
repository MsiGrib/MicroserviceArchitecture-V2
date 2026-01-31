using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Responses;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Responses;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Responses;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Features.Posts.Ui
{
    public partial class PostCard
    {
        [Parameter] public PostDTO Post { get; set; } = default!;
        [Parameter] public EventCallback<PostDTO> OnPostUpdated { get; set; }

        private async Task HandleCommentAdded(CommentDTO newComment)
        {
            var updatedPost = Post with
            {
                Comments = Post.Comments.Append(newComment).ToList(),
            };

            Post = updatedPost;
            await OnPostUpdated.InvokeAsync(updatedPost);
        }

        private async Task HandleCommentDeleted(Guid commentId)
        {
            var updatedPost = Post with
            {
                Comments = Post.Comments.Where(c => c.Id != commentId).ToList(),
            };

            Post = updatedPost;
            await OnPostUpdated.InvokeAsync(updatedPost);
        }

        private async Task HandleReactionAdded(ReactionDTO newReaction)
        {
            var updatedPost = Post with
            {
                Reactions = Post.Reactions.Append(newReaction).ToList(),
            };

            Post = updatedPost;
            await OnPostUpdated.InvokeAsync(updatedPost);
        }

        private async Task HandleReactionRemoved(Guid postId)
        {
            var updatedPost = Post with
            {
                Reactions = Post.Reactions.ToList(),
            };

            Post = updatedPost;
            await OnPostUpdated.InvokeAsync(updatedPost);
        }
    }
}