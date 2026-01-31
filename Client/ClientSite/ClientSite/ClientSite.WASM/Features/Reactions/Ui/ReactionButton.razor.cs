using Api.Interfaces;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Requests;
using Api.Models.MServices.ContentMService.Endpoints.ReactionEndpoints.Responses;
using ClientSite.WASM.Features.Reactions.Api;
using ClientSite.WASM.Features.Reactions.Models;
using ClientSite.WASM.Shared.Services;
using ClientSite.WASM.Shared.Storages.Lib;
using ClientSite.WASM.Shared.Storages.Model;
using Microsoft.AspNetCore.Components;

namespace ClientSite.WASM.Features.Reactions.Ui
{
    public partial class ReactionButton
    {
        #region Params

        [Parameter] public Guid PostId { get; set; }
        [Parameter] public List<ReactionDTO> Reactions { get; set; } = new();
        [Parameter] public EventCallback<ReactionDTO> OnReactionAdded { get; set; }
        [Parameter] public EventCallback<ReactionDTO> OnReactionRemoved { get; set; }

        #endregion

        #region Injects

        [Inject] private IClientStorage ClientStorage { get; init; } = default!;
        [Inject] private IAuthenticatedApiService AuthenticatedApiService { get; init; } = default!;
        [Inject] private IMicroservicesClient MicroservicesClient { get; init; } = default!;

        #endregion

        #region UI Fields

        private ReactionsApi? _api = null;
        private ClientSettings? _clientSettings = null;
        private bool _showReactionModal = false;

        #endregion

        #region LC Methods

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _api = new ReactionsApi(MicroservicesClient, AuthenticatedApiService);
            _clientSettings = await ClientStorage.GetClientSettingsAsync();
        }

        #endregion

        #region Private methods

        private List<ReactionGroup> GetGroupedReactions()
        {
            return Reactions
                .GroupBy(r => r.Type)
                .Select(g => new ReactionGroup
                {
                    Type = g.Key,
                    Count = g.Count(),
                    HasUserReaction = _clientSettings != null &&
                        g.Any(r => r.UserId == _clientSettings.UserId),
                    UserIds = g.Select(r => r.UserId).ToList()
                })
                .OrderByDescending(g => g.Count)
                .ThenBy(g => g.Type)
                .Take(6)
                .ToList();
        }

        private string GetReactionButtonClass(int reactionType, bool hasUserReaction)
        {
            var type = ReactionTypes.GetById(reactionType);
            var baseClass = "border ";

            if (hasUserReaction)
                return $"{baseClass} {type?.Color} border-current bg-opacity-10";

            return $"{baseClass} border-gray-300 bg-gray-50 hover:bg-gray-100";
        }

        private async Task ToggleReaction(int reactionType)
        {
            if (_clientSettings == null) return;

            try
            {
                var existingUserReaction = Reactions
                    .FirstOrDefault(r => r.UserId == _clientSettings.UserId);

                if (existingUserReaction != null)
                {
                    if (existingUserReaction.Type == reactionType)
                        await RemoveReaction(reactionType, existingUserReaction);
                    else
                        await UpdateReaction(reactionType, existingUserReaction);
                }
                else await AddReaction(reactionType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при работе с реакцией: {ex.Message}");
            }
        }

        private async Task AddReaction(int reactionType)
        {
            try
            {
                var request = new AddReactionRequest
                {
                    PostId = PostId,
                    ReactionType = reactionType
                };

                var reaction = await _api!.AddOrUpdateReaction(request);

                if (OnReactionAdded.HasDelegate)
                    await OnReactionAdded.InvokeAsync(reaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении реакции: {ex.Message}");
            }
        }

        private async Task UpdateReaction(int newReactionType, ReactionDTO existingReaction)
        {
            try
            {
                var request = new AddReactionRequest
                {
                    PostId = PostId,
                    ReactionType = newReactionType
                };

                var updatedReaction = await _api!.AddOrUpdateReaction(request);

                if (OnReactionRemoved.HasDelegate)
                    await OnReactionRemoved.InvokeAsync(existingReaction);
                if (OnReactionAdded.HasDelegate)
                    await OnReactionAdded.InvokeAsync(updatedReaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении реакции: {ex.Message}");
            }
        }

        private async Task RemoveReaction(int reactionType, ReactionDTO existingReaction)
        {
            try
            {
                await _api!.RemoveReaction(PostId);

                if (OnReactionRemoved.HasDelegate)
                    await OnReactionRemoved.InvokeAsync(existingReaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении реакции: {ex.Message}");
            }
        }

        private void ToggleReactionModal()
            => _showReactionModal = !_showReactionModal;

        private async Task SelectReaction(int reactionType)
        {
            await ToggleReaction(reactionType);
            _showReactionModal = false;
        }

        #endregion
    }
}