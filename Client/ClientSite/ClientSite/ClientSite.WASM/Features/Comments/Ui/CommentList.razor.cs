using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Responses;
using ClientSite.WASM.Features.Comments.Api;
using ClientSite.WASM.Shared.Services;
using ClientSite.WASM.Shared.Storages.Lib;
using ClientSite.WASM.Shared.Storages.Model;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Features.Comments.Ui
{
    public partial class CommentList
    {
        [Parameter] public Guid PostId { get; set; }
        [Parameter] public List<CommentDTO> Comments { get; set; } = new();
        [Parameter] public EventCallback<CommentDTO> OnCommentAdded { get; set; }
        [Parameter] public EventCallback<Guid> OnCommentDeleted { get; set; }

        [Inject] private IClientStorage ClientStorage { get; init; } = default!;
        [Inject] private IAuthenticatedApiService AuthenticatedApiService { get; init; } = default!;
        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;

        private CommentsApi? _api = null;
        private List<CommentDTO> _comments = new();
        private ClientSettings? _clientSettings = null;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _api = new CommentsApi(MicroservicesClient, AuthenticatedApiService);
            _clientSettings = await ClientStorage.GetClientSettingsAsync();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _comments = Comments;
        }

        private bool IsCurrentUser(Guid userId)
            => _clientSettings?.UserId == userId;

        private async Task DeleteComment(Guid commentId)
        {
            try
            {
                await _api!.DeleteComment(commentId);
                await OnCommentDeleted.InvokeAsync(commentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении комментария: {ex.Message}");
            }
        }
    }
}