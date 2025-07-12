using Application.DTO;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.CacheEntities;
using Domain.CacheEntities.Reactions;
using Domain.CoreEntities;
using Domain.Events;

namespace Application.Services
{
    public class ReactionNotificationService(IUnitOfWork unitOfWork1, IRealtimeNotifier _realtimeNotifier, IProfileServiceClient profileServiceClient)
        : IReactionNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork1;
        private readonly IRealtimeNotifier realtimeNotifier = _realtimeNotifier;
        private readonly IProfileServiceClient _profileServiceClient = profileServiceClient;

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
            var UserExist = await _unitOfWork.CacheRepository<UserSkeleton>().GetSingleAsync(i => i.UserId == reactionEventDTO.User.UserId);
            var isNewCache = false;
            var profileDTO = new ResponseWrapper<ProfileDTO>();
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
            if (UserExist == null)
            {
                // get userSkeleton 
                profileDTO = await _profileServiceClient.GetProfileAsync(reactionEventDTO.User.Id);
                // add userSkeleton to Cache 
                await _unitOfWork.CacheRepository<UserSkeleton>().AddAsync(UserExist = new UserSkeleton
                {
                    UserId = reactionEventDTO.User.UserId,
                    UserNames = reactionEventDTO.User.UserNames,
                    ProfileImageUrls = reactionEventDTO.User.ProfileImageUrls,
                });


            }
            else
            {
                profileDTO.Data = new ProfileDTO
                {
                    UserId = UserExist.UserId,
                    UserNames = UserExist.UserNames,
                    ProfileImageUrl = UserExist.ProfileImageUrls,

                };
            }
            ;


            UpdateCachedReactionList(cachedReaction, reactionEventDTO, profileDTO);

            if (isNewCache)
                await _unitOfWork.CacheRepository<CachedReactions>().AddAsync(cachedReaction);
            else
                await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(cachedReaction);

            // Send notification
            await realtimeNotifier.SendMessageAsync(authorId, new NotificationsDTO
            {
                EntityName = Domain.Enums.NotificationEntity.Comment,
                SourceUsername = reactionEventDTO.User.UserNames,
                SourceUserImageUrl = reactionEventDTO.User.ProfileImageUrls,
                CreatedTime = DateTime.UtcNow,
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
            return await _unitOfWork.CoreRepository<Reaction>().GetSingleAsync(i => i.AuthorId == authorEntityId);
        }

        private async Task<CachedReactions?> GetCachedUserReaction(string authorEntityId)
        {
            return await _unitOfWork.CacheRepository<CachedReactions>().GetSingleAsync(i => i.AuthorId == authorEntityId);
        }

        private void UpdateCachedReactionList(CachedReactions cache, ReactionEvent reactionEvent, ResponseWrapper<ProfileDTO> profile)
        {
            var userSkeleton = new UserSkeleton
            {
                Seen = false,
                CreatedAt = reactionEvent.User.CreatedAt,
                Id = reactionEvent.Id,
                UserId = profile.Data.UserId,
                ProfileImageUrls = profile.Data.ProfileImageUrl,
                UserNames = profile.Data.UserNames
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
            // Send notification
            await realtimeNotifier.SendMessageAsync(reactionEvent.AuthorEntityId, new NotificationsDTO
            {
                EntityName = Domain.Enums.NotificationEntity.Comment,
                SourceUsername = reactionEvent.User.UserNames,
                SourceUserImageUrl = reactionEvent.User.ProfileImageUrls,
                CreatedTime = DateTime.UtcNow,
            });
        }

        public Task RemovReactionsFromNotificationList(ReactionEvent ReactionEventDTO)
        {
            throw new NotImplementedException();
        }


    }
}
