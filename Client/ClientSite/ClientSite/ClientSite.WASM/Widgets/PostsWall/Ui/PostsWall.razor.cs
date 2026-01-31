using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Responses;
using ClientSite.WASM.Features.Posts.Api;
using ClientSite.WASM.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Widgets.PostsWall.Ui
{
    public partial class PostsWall
    {
        #region Injects

        [Inject] private IAuthenticatedApiService AuthenticatedApiService { get; init; } = default!;
        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;

        #endregion

        #region UI Fields

        private PostsApi? _api = null;
        private List<PostDTO> _posts = new();
        private bool _isLoading = true;
        private bool _isCreatingPost = false;

        #endregion

        #region LC Methods

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _api = new PostsApi(MicroservicesClient, AuthenticatedApiService);
            await LoadPosts();
        }

        #endregion

        #region Private methods

        private async Task LoadPosts()
        {
            try
            {
                _isLoading = true;
                StateHasChanged();

                _posts = await _api!.GetAllPosts();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке постов: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private void ShowCreatePostForm()
            => _isCreatingPost = true;

        private async Task HandlePostCreated(Guid postId)
        {
            _isCreatingPost = false;
            await LoadPosts();
        }

        private void HandlePostUpdated(PostDTO updatedPost)
        {
            var index = _posts.FindIndex(p => p.Id == updatedPost.Id);
            if (index >= 0)
            {
                _posts[index] = updatedPost;
                StateHasChanged();
            }
        }

        #endregion
    }
}