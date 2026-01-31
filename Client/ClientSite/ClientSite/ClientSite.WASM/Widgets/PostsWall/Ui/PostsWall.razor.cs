using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Responses;
using ClientSite.WASM.Features.Posts.Api;
using ClientSite.WASM.Shared.Services;
using ClientSite.WASM.Shared.Storages.Lib;
using ClientSite.WASM.Shared.Storages.Model;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Widgets.PostsWall.Ui
{
    public partial class PostsWall
    {
        [Inject] private IClientStorage ClientStorage { get; init; } = default!;
        [Inject] private IAuthenticatedApiService AuthenticatedApiService { get; init; } = default!;
        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;

        private PostsApi? _api = null;
        private List<PostDTO> _posts = new();
        private bool _isLoading = true;
        private ClientSettings? _clientSettings = null;
        private bool IsCreatingPost { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _api = new PostsApi(MicroservicesClient, AuthenticatedApiService);
            _clientSettings = await ClientStorage.GetClientSettingsAsync();
            await LoadPosts();
        }

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
        {
            IsCreatingPost = true;
        }

        private async Task HandlePostCreated(Guid postId)
        {
            IsCreatingPost = false;
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
    }
}