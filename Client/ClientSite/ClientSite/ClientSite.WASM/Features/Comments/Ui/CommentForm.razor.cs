using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.CommentEndpoints.Responses;
using ClientSite.WASM.Features.Comments.Api;
using ClientSite.WASM.Shared.Services;
using ClientSite.WASM.Shared.Storages.Lib;
using ClientSite.WASM.Shared.Storages.Model;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Features.Comments.Ui
{
    public partial class CommentForm
    {
        #region Params 

        [Parameter] public Guid PostId { get; set; }
        [Parameter] public EventCallback<CommentDTO> OnCommentAdded { get; set; }

        #endregion

        #region Injects

        [Inject] private IClientStorage ClientStorage { get; init; } = default!;
        [Inject] private IAuthenticatedApiService AuthenticatedApiService { get; init; } = default!;
        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;

        #endregion

        #region UI Fields

        private CommentsApi? _api = null;
        private ClientSettings? _clientSettings = null;
        private string _newCommentText = string.Empty;

        #endregion

        #region LC Methods

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _api = new CommentsApi(MicroservicesClient, AuthenticatedApiService);
        }

        #endregion

        #region Private methods

        private void OnContentHandler(string content)
            => _newCommentText = content;

        private async Task OnAddCommentClick()
        {
            if (string.IsNullOrWhiteSpace(_newCommentText))
                return;

            try
            {
                _clientSettings = await ClientStorage.GetClientSettingsAsync();

                if (_clientSettings == null)
                    return;

                var request = new CreateCommentRequest
                {
                    PostId = PostId,
                    Text = _newCommentText
                };

                var commentId = await _api!.CreateComment(request);

                var newComment = new CommentDTO
                {
                    Id = commentId,
                    Text = _newCommentText,
                    PostId = PostId,
                    UserId = _clientSettings.UserId,
                };

                _newCommentText = string.Empty;

                if (OnCommentAdded.HasDelegate)
                    await OnCommentAdded.InvokeAsync(newComment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении комментария: {ex.Message}");
            }
        }

        #endregion
    }
}