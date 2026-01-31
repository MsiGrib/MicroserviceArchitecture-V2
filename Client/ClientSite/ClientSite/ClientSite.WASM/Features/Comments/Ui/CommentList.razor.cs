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
        #region Params

        [Parameter] public List<CommentDTO> Comments { get; set; } = new();
        [Parameter] public EventCallback<Guid> OnCommentDeleted { get; set; }

        #endregion

        #region Injects

        [Inject] private IClientStorage ClientStorage { get; init; } = default!;
        [Inject] private IAuthenticatedApiService AuthenticatedApiService { get; init; } = default!;
        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;

        #endregion

        #region UI Fields

        private CommentsApi? _api = null;
        private List<CommentDTO> _comments = new();
        private ClientSettings? _clientSettings = null;

        #endregion

        #region LC Methods

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

        #endregion

        #region Private methods

        private bool IsCurrentUser(Guid userId)
            => _clientSettings?.UserId == userId;

        private async Task OnDeleteCommentClick(Guid commentId)
        {
            try
            {
                await _api!.DeleteComment(commentId);

                if (OnCommentDeleted.HasDelegate)
                    await OnCommentDeleted.InvokeAsync(commentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении комментария: {ex.Message}");
            }
        }

        #endregion
    }
}