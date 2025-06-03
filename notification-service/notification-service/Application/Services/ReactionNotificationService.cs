using Application.DTO;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.CacheEntities;
using Domain.CacheEntities.Reactions;
using Domain.CoreEntities;
using Domain.Enums;
using Domain.Events;

namespace Application.Services
{
    public class ReactionNotificationService(IUnitOfWork unitOfWork1) : IReactionNotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork1;
        public async Task UpdateReactionsListNotification(ReactionEvent ReactionEventDTO)
        {

            var CoreUsersReaction = _unitOfWork.CoreRepository<Reaction>().GetAsync(ReactionEventDTO.AuthorEntityId!);
            if (CoreUsersReaction == null)
                return;
            var CoreUsersReactionResult = await CoreUsersReaction;
            if (CoreUsersReactionResult == null)
                return;

            // Implementation for updating the Reactions list notification
            if (ReactionEventDTO.ReactedOn == ReactedEntity.Post)
                CoreUsersReactionResult.ReactionsOnPostId.Add(ReactionEventDTO.ReactionEntityId!);
            else if (ReactionEventDTO.ReactedOn == ReactedEntity.Comment)
                CoreUsersReactionResult.ReactionsOnCommentId.Add(ReactionEventDTO.ReactionEntityId!);
            else if (ReactionEventDTO.ReactedOn == ReactedEntity.Message)
                CoreUsersReactionResult.ReactionsOnCommentId.Add(ReactionEventDTO.ReactionEntityId!);

            var CacheUsersReaction = _unitOfWork.CacheRepository<CachedReactions>().GetAsync(ReactionEventDTO.AuthorEntityId!);

            if (CacheUsersReaction == null)
                return;

            var CacheUsersReactionResult = await CacheUsersReaction;
            if (CacheUsersReactionResult == null)
                return;

            ProfileDTO profileDTO = HelperRequestDataFromProfileService();
            if (ReactionEventDTO.ReactedOn == ReactedEntity.Post)
            {
                //request to get all the skelton data
                CacheUsersReactionResult.ReactionsOnPosts.Add(new ReactionPostDetails
                {
                    PostId = ReactionEventDTO.ReactionEntityId!,
                    ReactionId = ReactionEventDTO.Id!,
                    //this should be aggregated once every day as the content of the post may change
                    PostContent = ReactionEventDTO.Content ?? "",
                    ReactionType = ReactionEventDTO.Type,
                    User = new UserSkeleton
                    {
                        Seen = false,
                        CreatedAt = ReactionEventDTO.User.CreatedAt,
                        Id = ReactionEventDTO.Id,
                        UserId = profileDTO.UserId,
                        ProfileImageUrls = profileDTO.ProfileImageUrl,
                        UserNames = profileDTO.UserNames
                    }
                });

            }

            else if (ReactionEventDTO.ReactedOn == ReactedEntity.Comment)
            {
                CacheUsersReactionResult.ReactionsOnComments.Add(new ReactionCommentDetails
                {
                    CommentId = ReactionEventDTO.ReactionEntityId!,
                    ReactionId = ReactionEventDTO.Id!,
                    //this should be aggregated once every day as the content of the post may change
                    CommentContent = ReactionEventDTO.Content ?? "",
                    ReactionType = ReactionEventDTO.Type,
                    User = new UserSkeleton
                    {
                        Seen = false,
                        CreatedAt = ReactionEventDTO.User.CreatedAt,
                        Id = ReactionEventDTO.Id,
                        UserId = profileDTO.UserId,
                        ProfileImageUrls = profileDTO.ProfileImageUrl,
                        UserNames = profileDTO.UserNames
                    }
                });
            }

            else if (ReactionEventDTO.ReactedOn == ReactedEntity.Message)
            {
                CacheUsersReactionResult.ReactionMessageDetails.Add(new ReactionMessageDetails
                {
                    MessageId = ReactionEventDTO.ReactionEntityId!,
                    ReactionId = ReactionEventDTO.Id!,
                    //this should be aggregated once every day as the content of the post may change
                    MessageContent = ReactionEventDTO.Content ?? "",
                    ReactionType = ReactionEventDTO.Type,
                    User = new UserSkeleton
                    {
                        Seen = false,
                        CreatedAt = ReactionEventDTO.User.CreatedAt,
                        Id = ReactionEventDTO.Id,
                        UserId = profileDTO.UserId,
                        ProfileImageUrls = profileDTO.ProfileImageUrl,
                        UserNames = profileDTO.UserNames
                    }

                });
            }
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