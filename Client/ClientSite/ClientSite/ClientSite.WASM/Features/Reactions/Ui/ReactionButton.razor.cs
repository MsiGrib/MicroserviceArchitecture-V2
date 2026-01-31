using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Responses;
using ClientSite.WASM.Features.Reactions.Api;
using ClientSite.WASM.Shared.Services;
using ClientSite.WASM.Shared.Storages.Lib;
using ClientSite.WASM.Shared.Storages.Model;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Features.Reactions.Ui
{
    public partial class ReactionButton
    {
        [Parameter] public Guid PostId { get; set; }
        [Parameter] public List<ReactionDTO> Reactions { get; set; } = new();
        [Parameter] public EventCallback<ReactionDTO> OnReactionAdded { get; set; }
        [Parameter] public EventCallback<Guid> OnReactionRemoved { get; set; }

        [Inject] private IClientStorage ClientStorage { get; init; } = default!;
        [Inject] private IAuthenticatedApiService AuthenticatedApiService { get; init; } = default!;
        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;

        private ReactionsApi? _api = null;
        private ClientSettings? _clientSettings;
        private int ReactionCount => Reactions.Count;
        private bool HasCurrentUserReaction =>
            _clientSettings != null && Reactions.Any(r => r.UserId == _clientSettings.UserId);

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _api = new ReactionsApi(MicroservicesClient, AuthenticatedApiService);
            _clientSettings = await ClientStorage.GetClientSettingsAsync();
        }

        private async Task AddReaction()
        {
            if (_clientSettings == null) return;

            try
            {
                var request = new AddReactionRequest
                {
                    PostId = PostId,
                    ReactionType = 1,
                };

                var reaction = await _api!.AddOrUpdateReaction(request);
                await OnReactionAdded.InvokeAsync(reaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении реакции: {ex.Message}");
            }
        }

        private async Task RemoveReaction()
        {
            if (_clientSettings == null) return;

            try
            {
                await _api!.RemoveReaction(PostId);
                await OnReactionRemoved.InvokeAsync(PostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении реакции: {ex.Message}");
            }
        }
    }
}