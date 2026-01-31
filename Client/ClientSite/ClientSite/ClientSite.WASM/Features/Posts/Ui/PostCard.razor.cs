using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Responses;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Responses;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Responses;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Features.Posts.Ui
{
    public partial class PostCard
    {
        #region Params

        [Parameter] public PostDTO Post { get; set; } = default!;
        [Parameter] public EventCallback<PostDTO> OnPostUpdated { get; set; }

        #endregion

        #region Private methods

        private async Task HandleCommentAdded(CommentDTO newComment)
        {
            var updatedPost = Post with
            {
                Comments = Post.Comments.Append(newComment).ToList(),
            };

            Post = updatedPost;

            if (OnPostUpdated.HasDelegate)
                await OnPostUpdated.InvokeAsync(updatedPost);
        }

        private async Task HandleCommentDeleted(Guid commentId)
        {
            var updatedPost = Post with
            {
                Comments = Post.Comments.Where(c => c.Id != commentId).ToList(),
            };

            Post = updatedPost;

            if (OnPostUpdated.HasDelegate)
                await OnPostUpdated.InvokeAsync(updatedPost);
        }

        private async Task HandleReactionAdded(ReactionDTO newReaction)
        {
            var reactionsWithoutUser = Post.Reactions
                .Where(r => r.UserId != newReaction.UserId)
                .ToList();

            var updatedPost = Post with
            {
                Reactions = reactionsWithoutUser.Append(newReaction).ToList(),
            };

            Post = updatedPost;

            if (OnPostUpdated.HasDelegate)
                await OnPostUpdated.InvokeAsync(updatedPost);
        }

        private async Task HandleReactionRemoved(ReactionDTO removedReaction)
        {
            var updatedReactions = Post.Reactions
                .Where(r => r.UserId != removedReaction.UserId ||
                           (r.UserId == removedReaction.UserId && r.Type != removedReaction.Type))
                .ToList();

            var updatedPost = Post with
            {
                Reactions = updatedReactions,
            };

            Post = updatedPost;

            if (OnPostUpdated.HasDelegate)
                await OnPostUpdated.InvokeAsync(updatedPost);
        }

        #endregion
    }
}