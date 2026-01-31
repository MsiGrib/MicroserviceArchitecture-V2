using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Requests;
using ClientSite.WASM.Features.Posts.Api;
using ClientSite.WASM.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Features.Posts.Ui
{
    public partial class CreatePostForm
    {
        #region Params

        [Parameter] public EventCallback<Guid> OnPostCreated { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        #endregion

        #region Injects

        [Inject] private IAuthenticatedApiService AuthenticatedApiService { get; init; } = default!;
        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;

        #endregion

        #region UI Fields

        private PostsApi? _api = null;
        private string _title = string.Empty;
        private string _content = string.Empty;

        #endregion

        #region LC Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _api = new PostsApi(MicroservicesClient, AuthenticatedApiService);
        }

        #endregion

        #region Private methods

        private void OnTitleHandler(string title)
            => _title = title;

        private void OnContentHandler(string content)
            => _content = content;

        private async Task OnCreatePostClick()
        {
            if (string.IsNullOrWhiteSpace(_title) || string.IsNullOrWhiteSpace(_content))
                return;

            try
            {
                var request = new CreatePostRequest
                {
                    Title = _title,
                    Content = _content,
                };

                var postId = await _api!.CreatePost(request);

                if (OnPostCreated.HasDelegate)
                    await OnPostCreated.InvokeAsync(postId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании поста: {ex.Message}");
            }
        }

        private async Task OnCancelClick()
        {
            _title = string.Empty;
            _content = string.Empty;

            if (OnCancel.HasDelegate)
                await OnCancel.InvokeAsync();
        }

        #endregion
    }
}