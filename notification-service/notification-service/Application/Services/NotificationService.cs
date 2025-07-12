using Application.DTO;
using Application.Interfaces.Services;
using Application.Interfaces;
using Domain.CacheEntities.Comments;
using Domain.CacheEntities.Message;
using Domain.CacheEntities.Reactions;
using Domain.CacheEntities;
using Domain.CoreEntities;
using Domain.Enums;
using System.Linq.Expressions;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region GetNotifications
        public async Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetAllNotifications(string userId ,string next)
        {
            try
            {
                var notifications = new List<NotificationsDTO>();

                var followNotifications = await GetFollowNotification(userId,next);
                if (followNotifications.Data != null)
                    notifications.AddRange(followNotifications.Data);

                var commentNotifications = await GetCommentNotification(userId,next);
                if (commentNotifications.Data != null)
                    notifications.AddRange(commentNotifications.Data);

                var reactionNotifications = await GetReactionNotification(userId, next);
                if (reactionNotifications.Data != null)
                    notifications.AddRange(reactionNotifications.Data);

                var messageNotifications = await GetMessageNotifications(userId , next );
                if (messageNotifications.Data != null)
                    notifications.AddRange(messageNotifications.Data);

                if (notifications.Count == 0)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No notifications found"
                    };
                }

                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Data = notifications,
                    Message = "Notifications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Message = "Failed to retrieve notifications",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetFollowNotification(string userId , string next)
        {
            try
            {
                var notificationDto = new List<NotificationsDTO>();

                var notificationResult = await _unitOfWork.CacheRepository<CachedFollowed>().GetAsync(userId ,next);
                if (notificationResult == null)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No follow notifications found"
                    };
                }

                var followedNotifications = notificationResult.Followers;
                if (followedNotifications == null || followedNotifications.Count == 0)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No follow notifications found"
                    };
                }

                notificationDto = followedNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.ProfileImageUrls,
                    IsRead = x.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.UserId,
                    EntityName = NotificationEntity.Follow,
                    NotificationPreview = $"{x.UserNames} started following you.",
                    SourceUsername = x.UserNames
                }).ToList();

                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Data = notificationDto,
                    Message = "Follow notifications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Message = "Failed to retrieve follow notifications",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetCommentNotification(string userId ,string next)
        {
            try
            {
                var notificationDto = new List<NotificationsDTO>();

                var notificationResult = await _unitOfWork.CacheRepository<CachedCommentsNotification>().GetAsync(userId ,next);
                if (notificationResult == null)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No comment notifications found"
                    };
                }

                var commentNotifications = notificationResult.CommnetDetails;
                if (commentNotifications == null || commentNotifications.Count == 0)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No comment notifications found"
                    };
                }

                notificationDto = commentNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.CommentId,
                    EntityName = NotificationEntity.Comment,
                    NotificationPreview = $"{x.User.UserNames} commented on your post.",
                    SourceUsername = x.User.UserNames
                }).ToList();

                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Data = notificationDto,
                    Message = "Comment notifications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Message = "Failed to retrieve comment notifications",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetReactionNotification(string userId,string next)
        {
            try
            {
                var notificationDto = new List<NotificationsDTO>();

                var notificationResult = await _unitOfWork.CacheRepository<CachedReactions>().GetAsync(userId,next);
                if (notificationResult == null)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No reaction notifications found"
                    };
                }

                // Post reactions
                var postReactions = notificationResult.ReactionsOnPosts ?? new List<ReactionPostDetails>();
                var postReactionDtos = postReactions.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.ReactionId,
                    EntityName = NotificationEntity.React,
                    NotificationPreview = $"{x.User.UserNames} reacted to your post.",
                    SourceUsername = x.User.UserNames
                }).ToList();

                // Comment reactions
                var commentReactions = notificationResult.ReactionsOnComments ?? new List<ReactionCommentDetails>();
                var commentReactionDtos = commentReactions.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.ReactionId,
                    EntityName = NotificationEntity.React,
                    NotificationPreview = $"{x.User.UserNames} reacted to your comment.",
                    SourceUsername = x.User.UserNames
                }).ToList();

                // Message reactions
                var messageReactions = notificationResult.ReactionMessageDetails ?? new List<ReactionMessageDetails>();
                var messageReactionDtos = messageReactions.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.ReactionId,
                    EntityName = NotificationEntity.React,
                    NotificationPreview = $"{x.User.UserNames} reacted to your message.",
                    SourceUsername = x.User.UserNames
                }).ToList();

                notificationDto.AddRange(postReactionDtos);
                notificationDto.AddRange(commentReactionDtos);
                notificationDto.AddRange(messageReactionDtos);

                if (notificationDto.Count == 0)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No reaction notifications found"
                    };
                }

                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Data = notificationDto,
                    Message = "Reaction notifications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Message = "Failed to retrieve reaction notifications",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetMessageNotifications(string userId,string next)
        {
            try
            {
                var notificationDto = new List<NotificationsDTO>();

                var notificationResult = await _unitOfWork.CacheRepository<CachedMessage>().GetAsync(userId ,next);
                if (notificationResult == null)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No message notifications found"
                    };
                }

                var messageNotifications = notificationResult.Messages;
                if (messageNotifications == null || messageNotifications.Count == 0)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No message notifications found"
                    };
                }

                notificationDto = messageNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = x.User.CreatedAt,
                    EntityId = x.MessageId,
                    EntityName = NotificationEntity.Message,
                    NotificationPreview = $"{x.User.UserNames} sent you a message.",
                    SourceUsername = x.User.UserNames
                }).ToList();

                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Data = notificationDto,
                    Message = "Message notifications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Message = "Failed to retrieve message notifications",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }
        #endregion

        #region GetUnreadNotifications
        public async Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetAllUnseenNotification(string userId,string next)
        {
            try
            {
                var notifications = new List<NotificationsDTO>();

                var followNotifications = await GetUnreadFollowedNotifications(userId,next );
                if (followNotifications.Data != null)
                    notifications.AddRange(followNotifications.Data);

                var commentNotifications = await GetUnreadCommentNotifications(userId,next);
                if (commentNotifications.Data != null)
                    notifications.AddRange(commentNotifications.Data);

                var reactionNotifications = await GetUnreadReactionsNotifications(userId,next);
                if (reactionNotifications.Data != null)
                    notifications.AddRange(reactionNotifications.Data);

                var messageNotifications = await GetUnreadMessageNotifications(userId,next);
                if (messageNotifications.Data != null)
                    notifications.AddRange(messageNotifications.Data);

                if (notifications.Count == 0)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No unread notifications found"
                    };
                }

                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Data = notifications,
                    Message = "Unread notifications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Message = "Failed to retrieve unread notifications",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetUnreadFollowedNotifications(string userId,string next)
        {
            try
            {
                var notificationDto = new List<NotificationsDTO>();

                var notificationResult = await _unitOfWork.CacheRepository<CachedFollowed>().GetAsync(userId, next);
                if (notificationResult == null)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No unread follow notifications found"
                    };
                }

                var unreadNotifications = notificationResult.Followers?.Where(x => x.Seen == false).ToList();
                if (unreadNotifications == null || unreadNotifications.Count == 0)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No unread follow notifications found"
                    };
                }

                notificationDto = unreadNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.ProfileImageUrls,
                    IsRead = x.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.UserId,
                    EntityName = NotificationEntity.Follow,
                    NotificationPreview = $"{x.UserNames} started following you.",
                    SourceUsername = x.UserNames
                }).ToList();

                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Data = notificationDto,
                    Message = "Unread follow notifications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Message = "Failed to retrieve unread follow notifications",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetUnreadCommentNotifications(string userId,string next)
        {
            try
            {
                var notificationDto = new List<NotificationsDTO>();

                var notificationResult = await _unitOfWork.CacheRepository<CachedCommentsNotification>().GetAsync(userId ,next);
                if (notificationResult == null)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No unread comment notifications found"
                    };
                }

                var unreadNotifications = notificationResult.CommnetDetails?.Where(x => x.User.Seen == false).ToList();
                if (unreadNotifications == null || unreadNotifications.Count == 0)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No unread comment notifications found"
                    };
                }

                notificationDto = unreadNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.CommentId,
                    EntityName = NotificationEntity.Comment,
                    NotificationPreview = $"{x.User.UserNames} commented {x.Content[..Math.Min(20, x.Content.Length)]} on your post.",
                    SourceUsername = x.User.UserNames
                }).ToList();

                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Data = notificationDto,
                    Message = "Unread comment notifications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Message = "Failed to retrieve unread comment notifications",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetUnreadReactionsNotifications(string userId ,string next)
        {
            try
            {
                var notificationDto = new List<NotificationsDTO>();

                var notificationResult = await _unitOfWork.CacheRepository<CachedReactions>().GetAsync(userId,next);
                if (notificationResult == null)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No unread reaction notifications found"
                    };
                }

                // Post reactions
                var unreadPostReactions = notificationResult.ReactionsOnPosts?.Where(x => x.User.Seen == false).ToList() ?? new List<ReactionPostDetails>();
                var postReactionDtos = unreadPostReactions.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.PostId,
                    EntityName = NotificationEntity.React,
                    NotificationPreview = $"{x.User.UserNames} reacted to your post.",
                    SourceUsername = x.User.UserNames
                }).ToList();

                // Comment reactions
                var unreadCommentReactions = notificationResult.ReactionsOnComments?.Where(x => x.User.Seen == false).ToList() ?? new List<ReactionCommentDetails>();
                var commentReactionDtos = unreadCommentReactions.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = DateTime.Now,
                    EntityId = x.CommentId,
                    EntityName = NotificationEntity.React,
                    NotificationPreview = $"{x.User.UserNames} reacted to your comment.",
                    SourceUsername = x.User.UserNames
                }).ToList();

                notificationDto.AddRange(postReactionDtos);
                notificationDto.AddRange(commentReactionDtos);

                if (notificationDto.Count == 0)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No unread reaction notifications found"
                    };
                }

                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Data = notificationDto,
                    Message = "Unread reaction notifications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Message = "Failed to retrieve unread reaction notifications",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<PaginationResponseWrapper<List<NotificationsDTO>>> GetUnreadMessageNotifications(string userId ,string  next)
        {
            try
            {
                var notificationDto = new List<NotificationsDTO>();

                var notificationResult = await _unitOfWork.CacheRepository<CachedMessage>().GetAsync(userId,next);
                if (notificationResult == null)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No unread message notifications found"
                    };
                }

                var unreadNotifications = notificationResult.Messages?.Where(x => x.User.Seen == false).ToList();
                if (unreadNotifications == null || unreadNotifications.Count == 0)
                {
                    return new PaginationResponseWrapper<List<NotificationsDTO>>
                    {
                        Data = new List<NotificationsDTO>(),
                        Message = "No unread message notifications found"
                    };
                }

                notificationDto = unreadNotifications.Select(x => new NotificationsDTO
                {
                    SourceUserImageUrl = x.User.ProfileImageUrls,
                    IsRead = x.User.Seen,
                    CreatedTime = x.User.CreatedAt,
                    EntityId = x.MessageId,
                    EntityName = NotificationEntity.Message,
                    NotificationPreview = $"{x.User.UserNames} sent you a message.",
                    SourceUsername = x.User.UserNames
                }).ToList();

                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Data = notificationDto,
                    Message = "Unread message notifications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new PaginationResponseWrapper<List<NotificationsDTO>>
                {
                    Message = "Failed to retrieve unread message notifications",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }
        #endregion

        #region MarkNotificationsAsRead
        public async Task<ResponseWrapper<bool>> MarkNotificationsFollowAsRead(string userId, string userFollowedId)
        {
            try
            {
                var userCached = await _unitOfWork.CacheRepository<CachedFollowed>().GetSingleByIdAsync(userId);
                if (userCached == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in cache",
                        ErrorType = ErrorType.NotFound
                    };
                }

                var userFollowed = userCached.Followers.FirstOrDefault(i => i.UserId == userFollowedId);
                if (userFollowed == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "Follow notification not found",
                        ErrorType = ErrorType.NotFound
                    };
                }

                userFollowed.Seen = true;
                await _unitOfWork.CacheRepository<CachedFollowed>().UpdateAsync(userCached, userCached.UserId);

                var userCore = await _unitOfWork.CoreRepository<Follows>()
                    .GetSingleIncludingAsync(f => f.MyId == userId);
                if (userCore == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in core database",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (!userCore.FollowsNotifReadByAuthor.Any(r => r == userFollowedId))
                {
                    userCore.FollowsNotifReadByAuthor.Add(userFollowedId);
                    await _unitOfWork.CoreRepository<Follows>().UpdateAsync(userCore);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "Follow notification marked as read successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to mark follow notification as read",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<bool>> MarkNotificationsCommentAsRead(string userId, string commentId)
        {
            try
            {
                var userCached = await _unitOfWork.CacheRepository<CachedCommentsNotification>().GetSingleByIdAsync(userId);
                if (userCached == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in cache",
                        ErrorType = ErrorType.NotFound
                    };
                }

                var userComment = userCached.CommnetDetails.FirstOrDefault(i => i.CommentId == commentId);
                if (userComment == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "Comment notification not found",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (!userComment.User.Seen)
                {
                    userComment.User.Seen = true;
                    await _unitOfWork.CacheRepository<CachedCommentsNotification>().UpdateAsync(userCached, userCached.UserId);
                }

                var userCore = await _unitOfWork.CoreRepository<Comments>()
                    .GetSingleIncludingAsync(f => f.PostAuthorId == userId);
                if (userCore == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in core database",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (!userCore.CommentNotifReadByAuthor.Any(r => r == commentId))
                {
                    userCore.CommentNotifReadByAuthor.Add(commentId);
                    await _unitOfWork.CoreRepository<Comments>().UpdateAsync(userCore);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "Comment notification marked as read successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to mark comment notification as read",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<bool>> MarkNotificationsMessagesAsRead(string userId, string messageId)
        {
            try
            {
                var userCached = await _unitOfWork.CacheRepository<CachedMessage>().GetSingleByIdAsync(userId);
                if (userCached == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in cache",
                        ErrorType = ErrorType.NotFound
                    };
                }

                var message = userCached.Messages.FirstOrDefault(i => i.MessageId == messageId);
                if (message == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "Message notification not found",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (!message.User.Seen)
                {
                    message.User.Seen = true;
                    await _unitOfWork.CacheRepository<CachedMessage>().UpdateAsync(userCached, userCached.RecieverUserId);
                }

                var userCore = await _unitOfWork.CoreRepository<Messages>()
                    .GetSingleIncludingAsync(f => f.RevieverId == userId);
                if (userCore == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in core database",
                        ErrorType = ErrorType.NotFound
                    };
                }

                foreach (var item in userCore.MessageList.Values)
                {
                    var msg = item.FirstOrDefault(x => x.Id == messageId);
                    if (msg != null)
                    {
                        msg.IsRead = true;
                        break;
                    }
                }

                await _unitOfWork.CoreRepository<Messages>().UpdateAsync(userCore);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "Message notification marked as read successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to mark message notification as read",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<bool>> MarkNotificationsReactionPostAsRead(string userId, string reactionId)
        {
            try
            {
                var userCached = await _unitOfWork.CacheRepository<CachedReactions>().GetSingleByIdAsync(userId);
                if (userCached == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in cache",
                        ErrorType = ErrorType.NotFound
                    };
                }

                var userReactPost = userCached.ReactionsOnPosts.FirstOrDefault(i => i.ReactionId == reactionId);
                if (userReactPost == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "Post reaction notification not found",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (!userReactPost.User.Seen)
                {
                    userReactPost.User.Seen = true;
                    await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(userCached, userCached.AuthorId);
                }

                var userCore = await _unitOfWork.CoreRepository<Reaction>()
                    .GetSingleIncludingAsync(f => f.AuthorId == userId);
                if (userCore == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in core database",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (!userCore.PostReactionsNotifReadByAuthor.Any(r => r == reactionId))
                {
                    userCore.PostReactionsNotifReadByAuthor.Add(reactionId);
                    await _unitOfWork.CoreRepository<Reaction>().UpdateAsync(userCore);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "Post reaction notification marked as read successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to mark post reaction notification as read",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<bool>> MarkNotificationsReactionMessagesAsRead(string userId, string reactionId)
        {
            try
            {
                var userCached = await _unitOfWork.CacheRepository<CachedReactions>().GetSingleByIdAsync(userId);
                if (userCached == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in cache",
                        ErrorType = ErrorType.NotFound
                    };
                }

                var userReactMessage = userCached.ReactionMessageDetails.FirstOrDefault(i => i.ReactionId == reactionId);
                if (userReactMessage == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "Message reaction notification not found",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (!userReactMessage.User.Seen)
                {
                    userReactMessage.User.Seen = true;
                    await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(userCached, userCached.AuthorId);
                }

                var userCore = await _unitOfWork.CoreRepository<Reaction>()
                    .GetSingleIncludingAsync(f => f.AuthorId == userId);
                if (userCore == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in core database",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (!userCore.MessageReactionsNotifReadByAuthor.Any(r => r == reactionId))
                {
                    userCore.MessageReactionsNotifReadByAuthor.Add(reactionId);
                    await _unitOfWork.CoreRepository<Reaction>().UpdateAsync(userCore);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "Message reaction notification marked as read successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to mark message reaction notification as read",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<bool>> MarkNotificationsReactionCommentAsRead(string userId, string reactionId)
        {
            try
            {
                var userCached = await _unitOfWork.CacheRepository<CachedReactions>().GetSingleByIdAsync(userId);
                if (userCached == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in cache",
                        ErrorType = ErrorType.NotFound
                    };
                }

                var userReactComment = userCached.ReactionsOnComments.FirstOrDefault(i => i.ReactionId == reactionId);
                if (userReactComment == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "Comment reaction notification not found",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (!userReactComment.User.Seen)
                {
                    userReactComment.User.Seen = true;
                    await _unitOfWork.CacheRepository<CachedReactions>().UpdateAsync(userCached, userCached.AuthorId);
                }

                var userCore = await _unitOfWork.CoreRepository<Reaction>()
                    .GetSingleIncludingAsync(f => f.AuthorId == userId);
                if (userCore == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in core database",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (!userCore.CommentReactionsNotifReadByAuthor.Any(r => r == reactionId))
                {
                    userCore.CommentReactionsNotifReadByAuthor.Add(reactionId);
                    await _unitOfWork.CoreRepository<Reaction>().UpdateAsync(userCore);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "Comment reaction notification marked as read successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to mark comment reaction notification as read",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }
        #endregion

        #region MarkAllNotificationsAsRead
        public async Task<ResponseWrapper<bool>> MarkAllNotificationsAsRead(string userId)
        {
            try
            {
                var results = await Task.WhenAll(
                     MarkAllNotificationsCommentAsRead(userId),
                    MarkAllNotificationsFollowAsRead(userId),
                    MarkAllNotificationsReactionsAsRead(userId)
                );

                if (results.Any(r => !r.Data))
                {
                    var errors = results.Where(r => !r.Data)
                                        .SelectMany(r => r.Errors ?? new List<string>())
                                        .ToList();

                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "Some notifications could not be marked as read",
                        Errors = errors,
                        ErrorType = errors.Any() ? ErrorType.InternalServerError : ErrorType.None
                    };
                }

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "All notifications marked as read successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to mark all notifications as read",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<bool>> MarkAllNotificationsReactionsAsRead(string userId)
        {
            try
            {
                // Cache update
                var userCached = await _unitOfWork.CacheRepository<CachedReactions>()
                    .GetSingleAsync(x => x.AuthorId == userId);
                if (userCached == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in cache",
                        ErrorType = ErrorType.NotFound
                    };
                }

                bool anyUnread = false;

                // Mark post reactions as read
                foreach (var reaction in userCached.ReactionsOnPosts ?? Enumerable.Empty<ReactionPostDetails>())
                {
                    if (!reaction.User.Seen)
                    {
                        reaction.User.Seen = true;
                        anyUnread = true;
                    }
                }

                // Mark comment reactions as read
                foreach (var reaction in userCached.ReactionsOnComments ?? Enumerable.Empty<ReactionCommentDetails>())
                {
                    if (!reaction.User.Seen)
                    {
                        reaction.User.Seen = true;
                        anyUnread = true;
                    }
                }

                // Mark message reactions as read
                foreach (var reaction in userCached.ReactionMessageDetails ?? Enumerable.Empty<ReactionMessageDetails>())
                {
                    if (!reaction.User.Seen)
                    {
                        reaction.User.Seen = true;
                        anyUnread = true;
                    }
                }

                if (anyUnread)
                {
                    await _unitOfWork.CacheRepository<CachedReactions>()
                        .UpdateAsync(userCached, userCached.AuthorId);
                }

                // Core DB update
                var userCore = await _unitOfWork.CoreRepository<Reaction>()
                    .GetSingleIncludingAsync(r => r.AuthorId == userId);
                if (userCore == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in core database",
                        ErrorType = ErrorType.NotFound
                    };
                }

                bool needsUpdate = false;

                // Add post reactions to read list
                foreach (var reaction in userCached.ReactionsOnPosts ?? Enumerable.Empty<ReactionPostDetails>())
                {
                    if (!userCore.PostReactionsNotifReadByAuthor.Any(r => r == reaction.ReactionId))
                    {
                        userCore.PostReactionsNotifReadByAuthor.Add(reaction.ReactionId);
                        needsUpdate = true;
                    }
                }

                // Add comment reactions to read list
                foreach (var reaction in userCached.ReactionsOnComments ?? Enumerable.Empty<ReactionCommentDetails>())
                {
                    if (!userCore.CommentReactionsNotifReadByAuthor.Any(r => r == reaction.ReactionId))
                    {
                        userCore.CommentReactionsNotifReadByAuthor.Add(reaction.ReactionId);
                        needsUpdate = true;
                    }
                }

                // Add message reactions to read list
                foreach (var reaction in userCached.ReactionMessageDetails ?? Enumerable.Empty<ReactionMessageDetails>())
                {
                    if (!userCore.MessageReactionsNotifReadByAuthor.Any(r => r == reaction.ReactionId))
                    {
                        userCore.MessageReactionsNotifReadByAuthor.Add(reaction.ReactionId);
                        needsUpdate = true;
                    }
                }

                if (needsUpdate)
                {
                    await _unitOfWork.CoreRepository<Reaction>().UpdateAsync(userCore);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "All reaction notifications marked as read successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to mark all reaction notifications as read",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<bool>> MarkAllNotificationsFollowAsRead(string userId)
        {
            try
            {
                // Cache update
                var userCached = await _unitOfWork.CacheRepository<CachedFollowed>()
                    .GetSingleAsync(x => x.UserId == userId);

                if (userCached == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in cache",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (userCached.Followers == null || userCached.Followers.Count == 0)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = true,
                        Message = "No follow notifications to mark as read"
                    };
                }

                bool anyUnread = false;
                foreach (var notification in userCached.Followers)
                {
                    if (!notification.Seen)
                    {
                        notification.Seen = true;
                        anyUnread = true;
                    }
                }

                if (anyUnread)
                {
                    await _unitOfWork.CacheRepository<CachedFollowed>()
                        .UpdateAsync(userCached, userCached.UserId);
                }

         
                var userCore = await _unitOfWork.CoreRepository<Follows>()
                    .GetSingleAsync(f => f.MyId == userId);

                if (userCore == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in core database",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (userCore.FollowersId == null || userCore.FollowersId.Count == 0)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = true,
                        Message = "No followers to process"
                    };
                }

                bool needsUpdate = false;
                foreach (var userFollowedId in userCore.FollowersId)
                {
                    if (!userCore.FollowsNotifReadByAuthor.Any(r => r == userFollowedId))
                    {
                        userCore.FollowsNotifReadByAuthor.Add(userFollowedId);
                        needsUpdate = true;
                    }
                }

                if (needsUpdate)
                {
                    await _unitOfWork.CoreRepository<Follows>().UpdateAsync(userCore);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "All follow notifications marked as read successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to mark all follow notifications as read",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<bool>> MarkAllNotificationsCommentAsRead(string userId)
        {
            try
            {
                // Cache update
                var userCached = await _unitOfWork.CacheRepository<CachedCommentsNotification>()
                    .GetSingleAsync(x => x.UserId == userId);

                if (userCached == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in cache",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (userCached.CommnetDetails == null || userCached.CommnetDetails.Count == 0)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = true,
                        Message = "No comment notifications to mark as read"
                    };
                }

                bool anyUnread = false;
                foreach (var comment in userCached.CommnetDetails)
                {
                    if (!comment.User.Seen)
                    {
                        comment.User.Seen = true;
                        anyUnread = true;
                    }
                }

                if (anyUnread)
                {
                    await _unitOfWork.CacheRepository<CachedCommentsNotification>()
                        .UpdateAsync(userCached, userCached.UserId);
                }



                var userCore = await _unitOfWork.CoreRepository<Comments>()
                    .GetSingleAsync(c => c.Id == userId);

                if (userCore == null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "User not found in core database",
                        ErrorType = ErrorType.NotFound
                    };
                }

                if (userCore.UserID_CommentIds == null || userCore.UserID_CommentIds.Count == 0)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = true,
                        Message = "No comments to process"
                    };
                }

                // Get all unique comment IDs from dictionaries
                var allCommentIds = userCore.UserID_CommentIds
                    .SelectMany(d => d.Value)
                    .ToList();

                // Find only new unread comments
                var newReadComments = allCommentIds
                    .Except(userCore.CommentNotifReadByAuthor ?? Enumerable.Empty<string>())
                    .ToList();

                if (newReadComments.Count > 0)
                {
                    userCore.CommentNotifReadByAuthor ??= new List<string>();
                    userCore.CommentNotifReadByAuthor.AddRange(newReadComments);

                    await _unitOfWork.CoreRepository<Comments>().UpdateAsync(userCore);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "All comment notifications marked as read successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to mark all comment notifications as read",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<List<NotificationEntity>>> GetNotificationTypes()
        {
            try
            {
                var types = Enum.GetValues(typeof(NotificationEntity))
                                .Cast<NotificationEntity>()
                                .ToList();

                return new ResponseWrapper<List<NotificationEntity>>
                {
                    Data = types,
                    Message = "Notification types retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<List<NotificationEntity>>
                {
                    Message = "Failed to retrieve notification types",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

  
       
        #endregion
    }

}
