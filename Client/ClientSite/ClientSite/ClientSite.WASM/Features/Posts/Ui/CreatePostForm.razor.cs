using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.PostEndpoints.Requests;
using ClientSite.WASM.Features.Posts.Api;
using ClientSite.WASM.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Features.Posts.Ui
{
    public partial class CreatePostForm
    {
        [Parameter] public EventCallback<Guid> OnPostCreated { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        [Inject] private IAuthenticatedApiService AuthenticatedApiService { get; init; } = default!;
        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;

        private PostsApi? _api = null;
        private CreatePostRequest NewPost { get; set; } = new();
        private bool CanCreatePost =>
            !string.IsNullOrWhiteSpace(NewPost.Title) &&
            !string.IsNullOrWhiteSpace(NewPost.Content);

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _api = new PostsApi(MicroservicesClient, AuthenticatedApiService);
        }

        private async Task CreatePost()
        {
            try
            {
                var postId = await _api!.CreatePost(NewPost);
                NewPost = new CreatePostRequest();
                await OnPostCreated.InvokeAsync(postId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании поста: {ex.Message}");
            }
        }

        private async Task Cancel()
        {
            NewPost = new CreatePostRequest();
            await OnCancel.InvokeAsync();
        }
    }
}