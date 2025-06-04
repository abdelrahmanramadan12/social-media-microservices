using Application.DTO;
using Application.Hubs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.CacheEntities;
using Domain.CacheEntities.Reactions;
using Domain.CoreEntities;
using Domain.Events;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services
{
    public class ReactionNotificationService(IUnitOfWork unitOfWork1, IHubContext<ReactionNotificationHub> hubContext)
     : IReactionNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork1;
        private readonly IHubContext<ReactionNotificationHub> _hubContext = hubContext;

        public async Task UpdateReactionsListNotification(ReactionEvent ReactionEventDTO)
        {
            var coreUserReaction = await GetCoreUserReaction(ReactionEventDTO.AuthorEntityId!);
            if (coreUserReaction == null) return;

            UpdateCoreUserReactionList(coreUserReaction, ReactionEventDTO);

            var cachedUserReactions = await GetCachedUserReaction(ReactionEventDTO.AuthorEntityId!);
            if (cachedUserReactions == null) return;

            var profileDTO = HelperRequestDataFromProfileService();

            UpdateCachedReactionList(cachedUserReactions, ReactionEventDTO, profileDTO);

            await NotifyUserAsync(ReactionEventDTO);
        }
        private async Task<Reaction?> GetCoreUserReaction(string authorEntityId)
        {
            var coreUserReactionTask = _unitOfWork.CoreRepository<Reaction>().GetAsync(authorEntityId);
            return coreUserReactionTask == null ? null : await coreUserReactionTask;
        }

        private void UpdateCoreUserReactionList(Reaction coreUserReaction, ReactionEvent reactionEvent)
        {
            switch (reactionEvent.ReactedOn)
            {
                case ReactedEntity.Post:
                    coreUserReaction.ReactionsOnPostId.Add(reactionEvent.ReactionEntityId!);
                    break;
                case ReactedEntity.Comment:
                case ReactedEntity.Message:
                    coreUserReaction.ReactionsOnCommentId.Add(reactionEvent.ReactionEntityId!);
                    break;
            }
        }

        private async Task<CachedReactions?> GetCachedUserReaction(string authorEntityId)
        {
            var cacheReactionTask = _unitOfWork.CacheRepository<CachedReactions>().GetAsync(authorEntityId);
            return cacheReactionTask == null ? null : await cacheReactionTask;
        }

        private void UpdateCachedReactionList(CachedReactions cache, ReactionEvent reactionEvent, ProfileDTO profile)
        {
            var userSkeleton = new UserSkeleton
            {
                Seen = false,
                CreatedAt = reactionEvent.User.CreatedAt,
                Id = reactionEvent.Id,
                UserId = profile.UserId,
                ProfileImageUrls = profile.ProfileImageUrl,
                UserNames = profile.UserNames
            };

            switch (reactionEvent.ReactedOn)
            {
                case ReactedEntity.Post:
                    cache.ReactionsOnPosts.Add(new ReactionPostDetails
                    {
                        PostId = reactionEvent.ReactionEntityId!,
                        ReactionId = reactionEvent.Id!,
                        PostContent = reactionEvent.Content ?? "",
                        ReactionType = reactionEvent.Type,
                        User = userSkeleton
                    });
                    break;

                case ReactedEntity.Comment:
                    cache.ReactionsOnComments.Add(new ReactionCommentDetails
                    {
                        CommentId = reactionEvent.ReactionEntityId!,
                        ReactionId = reactionEvent.Id!,
                        CommentContent = reactionEvent.Content ?? "",
                        ReactionType = reactionEvent.Type,
                        User = userSkeleton
                    });
                    break;

                case ReactedEntity.Message:
                    cache.ReactionMessageDetails.Add(new ReactionMessageDetails
                    {
                        MessageId = reactionEvent.ReactionEntityId!,
                        ReactionId = reactionEvent.Id!,
                        MessageContent = reactionEvent.Content ?? "",
                        ReactionType = reactionEvent.Type,
                        User = userSkeleton
                    });
                    break;
            }
        }

        private async Task NotifyUserAsync(ReactionEvent reactionEvent)
        {
            await _hubContext.Clients.User(reactionEvent.AuthorEntityId)
                .SendAsync("ReceiveReactionNotification", new
                {
                    ReactionId = reactionEvent.Id,
                    ReactedOn = reactionEvent.ReactedOn.ToString(),
                    Content = reactionEvent.Content,
                    EntityId = reactionEvent.ReactionEntityId,
                    User = new
                    {
                        reactionEvent.User.Id,
                        reactionEvent.User.UserId,
                        reactionEvent.User.UserNames,
                        reactionEvent.User.ProfileImageUrls,
                        reactionEvent.User.CreatedAt
                    }
                });
        }

        public Task RemovReactionsFromNotificationList(ReactionEvent ReactionEventDTO)
        {
            // Implementation for removing Reactions from the notification list
            throw new NotImplementedException();
        }

        private ProfileDTO HelperRequestDataFromProfileService()
        {
            var profile = new ProfileDTO();

            return profile;
        }
    }

}