using Application.DTO;
using Application.Hubs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.CacheEntities;
using Domain.CacheEntities.Reactions;
using Domain.CoreEntities;
using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReactionNotificationService(IUnitOfWork unitOfWork1, IHubContext<ReactionNotificationHub> hubContext)
        : IReactionNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork1;
        private readonly IHubContext<ReactionNotificationHub> _hubContext = hubContext;

        public async Task UpdateReactionsListNotification(ReactionEvent reactionEventDTO)
        {
            var authorId = reactionEventDTO.AuthorEntityId!;
            var entityId = reactionEventDTO.ReactionEntityId!;

            // Get or create core user reaction
            var coreUserReaction = await GetCoreUserReaction(authorId);

            if (coreUserReaction == null)
            {
                coreUserReaction = new Reaction
                {
                    AuthorId = authorId,
                    ReactionsOnPostId = new List<string>(),
                    ReactionsOnCommentId = new List<string>(),
                    ReactionsOnMessageId = new List<string>()
                };

                AddReactionToAppropriateList(coreUserReaction, reactionEventDTO.ReactedOn, entityId);
                await _unitOfWork.CoreRepository<Reaction>().AddAsync(coreUserReaction);
            }
            else
            {
                AddReactionIfNotExists(coreUserReaction, reactionEventDTO.ReactedOn, entityId);
                await _unitOfWork.CoreRepository<Reaction>().UpdateAsync(coreUserReaction);
            }

            // Handle cached reactions
            var cachedReaction = await GetCachedUserReaction(authorId);
            var isNewCache = false;

            if (cachedReaction == null)
            {
                cachedReaction = new CachedReactions
                {
                    AuthorId = authorId,
                    ReactionsOnPosts = new List<ReactionPostDetails>(),
                    ReactionsOnComments = new List<ReactionCommentDetails>(),
                    ReactionMessageDetails = new List<ReactionMessageDetails>()
                };
                isNewCache = true;
            }

            var profileDTO = HelperRequestDataFromProfileService();
            UpdateCachedReactionList(cachedReaction, reactionEventDTO, profileDTO);

            if (isNewCache)
                await _unitOfWork.CacheRepository<CachedReactions>().AddAsync(cachedReaction);
            else
                await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(cachedReaction);

            // use hub context 
           await  _hubContext.Clients.User(authorId)
                .SendAsync("ReceiveReactionNotification", new
                {
                    reactionEventDTO.Id,
                    reactionEventDTO.ReactedOn,
                    reactionEventDTO.Content,
                    reactionEventDTO.ReactionEntityId,
                    User = new
                    {
                        reactionEventDTO.User.Id,
                        reactionEventDTO.User.UserId,
                        reactionEventDTO.User.UserNames,
                        reactionEventDTO.User.ProfileImageUrls,
                        reactionEventDTO.User.CreatedAt
                    }
                });
            await NotifyUserAsync(reactionEventDTO);
        }

        private void AddReactionToAppropriateList(Reaction reaction, ReactedEntity reactedOn, string entityId)
        {
            switch (reactedOn)
            {
                case ReactedEntity.Post:
                    reaction.ReactionsOnPostId.Add(entityId);
                    break;
                case ReactedEntity.Comment:
                    reaction.ReactionsOnCommentId.Add(entityId);
                    break;
                case ReactedEntity.Message:
                    reaction.ReactionsOnMessageId.Add(entityId);
                    break;
            }
        }

        private void AddReactionIfNotExists(Reaction reaction, ReactedEntity reactedOn, string entityId)
        {
            switch (reactedOn)
            {
                case ReactedEntity.Post:
                    reaction.ReactionsOnPostId ??= new List<string>();
                    if (!reaction.ReactionsOnPostId.Contains(entityId))
                        reaction.ReactionsOnPostId.Add(entityId);
                    break;

                case ReactedEntity.Comment:
                    reaction.ReactionsOnCommentId ??= new List<string>();
                    if (!reaction.ReactionsOnCommentId.Contains(entityId))
                        reaction.ReactionsOnCommentId.Add(entityId);
                    break;

                case ReactedEntity.Message:
                    reaction.ReactionsOnMessageId ??= new List<string>();
                    if (!reaction.ReactionsOnMessageId.Contains(entityId))
                        reaction.ReactionsOnMessageId.Add(entityId);
                    break;
            }
        }

        private async Task<Reaction?> GetCoreUserReaction(string authorEntityId)
        {
            return await _unitOfWork.CoreRepository<Reaction>().GetSingleAsync(i=>i.AuthorId ==  authorEntityId);
        }

        private async Task<CachedReactions?> GetCachedUserReaction(string authorEntityId)
        {
            return await _unitOfWork.CacheRepository<CachedReactions>().GetSingleAsync(i=>i.AuthorId == authorEntityId);
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
            throw new NotImplementedException();
        }

        private ProfileDTO HelperRequestDataFromProfileService()
        {
            // TODO: Replace with actual call to Profile service
            return new ProfileDTO
            {
                UserId = "sample-user-id",
                ProfileImageUrl = "https://cdn.example.com/avatar.jpg",
                UserNames = "Sample User"
            };
        }
    }
}
