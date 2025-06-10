using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using MongoDB.Bson;
using Service.DTOs.Requests;
using Service.DTOs.Responses;
using Service.Enums;
using Service.Events;
using Service.Interfaces.CommentServices;
using Service.Interfaces.MediaServices;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.CommentServices
{
    public class CommentService : ICommentService
    {
        private const int MaxContentLength = 500;

        private static ResponseWrapper<CommentResponse>? ValidateContent(string content)
        {
            if (content.Length > MaxContentLength)
            {
                return new ResponseWrapper<CommentResponse>
                {
                    Message = "Validation error",
                    ErrorType = ErrorType.Validation,
                    Errors = new List<string> { $"Content cannot exceed {MaxContentLength} characters" }
                };
            }
            return null;
        }
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentPublisher _commentPublisher;
        private readonly IMediaServiceClient _mediaServiceClient;

        public CommentService(
            ICommentRepository commentRepository,
            IPostRepository postRepository,
            ICommentPublisher commentPublisher,
            IMediaServiceClient mediaServiceClient)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _commentPublisher = commentPublisher;
            _mediaServiceClient = mediaServiceClient;
        }

        public async Task<ResponseWrapper<PagedCommentsResponse>> ListCommentsAsync(GetPagedCommentRequest request)
        {
            try
            {
                var PageSize = 10;
                string? decryptedCursor = null;
                if (!string.IsNullOrWhiteSpace(request.Next))
                {
                    try
                    {
                        decryptedCursor = request.Next;
                    }
                    catch
                    {
                        decryptedCursor = null;
                    }
                }

                var comments = (await _commentRepository
                                    .GetByPostIdCursorAsync(request.PostId, decryptedCursor, PageSize))
                                    .ToList();

                string? nextCursor =
                    comments.Count < PageSize
                        ? null
                        : comments.Last().Id.ToString();

                var dto = new PagedCommentsResponse();
                dto.AddRange(comments.Select(ToDto));

                return new ResponseWrapper<PagedCommentsResponse>
                {
                    Data = dto,
                    Message = "Comments retrieved successfully",
                    Pagination = new PaginationMetadata
                    {
                        Next = nextCursor
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<PagedCommentsResponse>
                {
                    Message = "Failed to retrieve comments",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<CommentResponse>> CreateCommentAsync(CreateCommentRequest dto)
        {
            try
            {
                // Validate content length
                var contentValidation = ValidateContent(dto.Content);
                if (contentValidation != null)
                {
                    return contentValidation;
                }
                if (string.IsNullOrWhiteSpace(dto.PostId))
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "CommentId is required",
                        ErrorType = ErrorType.Validation,
                        Errors = new List<string> { "CommentId is required" }
                    };
                }

                // Validate media if present
                if (dto.HasMedia && dto.Media != null)
                {
                    if (dto.MediaType == MediaType.UNKNOWN)
                    {
                        return new ResponseWrapper<CommentResponse>
                        {
                            Message = "Invalid media type",
                            ErrorType = ErrorType.Validation,
                            Errors = new List<string> { "A valid media type must be specified when including media" }
                        };
                    }
                }

                // Check if post exists and is not deleted
                var post = await _postRepository.GetPostByIdAsync(dto.PostId);
                if (post == null || post.IsDeleted)
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "Post has been deleted or doesn't exist",
                        ErrorType = ErrorType.NotFound,
                        Errors = new List<string> { "Post has been deleted or doesn't exist" }
                    };
                }
                // Privacy filtration for update
                if (post.Privacy == Privacy.OnlyMe && post.AuthorId != dto.UserId)
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "You are not allowed to update a comment on this post",
                        ErrorType = ErrorType.UnAuthorized,
                        Errors = new List<string> { "You are not allowed to update a comment on this post" }
                    };
                }
                // If privacy is OnlyMe, ensure commentAuthor == postAuthor
                if (post.Privacy == Privacy.OnlyMe && post.AuthorId != dto.UserId)
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "You are not allowed to comment on this post",
                        ErrorType = ErrorType.UnAuthorized,
                        Errors = new List<string> { "You are not allowed to comment on this post" }
                    };
                }

                var comment = new Comment
                {
                    Id = ObjectId.GenerateNewId(),
                    PostId = dto.PostId,
                    AuthorId = dto?.UserId ?? "",
                    Content = dto?.Content ?? "",
                    CreatedAt = DateTime.Now,
                    IsEdited = false,
                    ReactCount = 0,
                    MediaUrl = ""
                };

                // Handle media upload if present
                if (dto.HasMedia && dto.Media != null)
                {
                    try
                    {
                        var mediaResponse = await _mediaServiceClient.AssignMediaToPostInput(dto);
                        
                        if (mediaResponse == null)
                        {
                            return new ResponseWrapper<CommentResponse>
                            {
                                Message = "Failed to upload media",
                                ErrorType = ErrorType.InternalServerError,
                                Errors = new List<string> { "Media service did not return a valid response" }
                            };
                        }
                        
                        if (!mediaResponse.Success)
                        {
                            return new ResponseWrapper<CommentResponse>
                            {
                                Message = "Failed to upload media",
                                ErrorType = ErrorType.InternalServerError,
                                Errors = new List<string> { "Media service returned an error" }
                            };
                        }
                        
                        var mediaUrl = mediaResponse.Urls?.FirstOrDefault();
                        if (string.IsNullOrEmpty(mediaUrl))
                        {
                            return new ResponseWrapper<CommentResponse>
                            {
                                Message = "Failed to upload media",
                                ErrorType = ErrorType.InternalServerError,
                                Errors = new List<string> { "No media URL was returned from the media service" }
                            };
                        }
                        
                        comment.MediaUrl = mediaUrl;
                        Console.WriteLine(mediaUrl);
                    }
                    catch (Exception ex)
                    {
                        return new ResponseWrapper<CommentResponse>
                        {
                            Message = "Failed to upload media",
                            ErrorType = ErrorType.InternalServerError,
                            Errors = new List<string> { $"An error occurred while uploading media: {ex.Message}" }
                        };
                    }
                }

                await _commentRepository.CreateAsync(comment);

                await _commentPublisher.PublishAsync(new CommentEvent
                {
                    EventType = EventType.Create,
                    CommentId = comment.Id.ToString(),
                    PostId = comment.PostId,
                    CommentAuthorId = comment.AuthorId,
                    Content = comment.Content,
                    MediaUrl = comment.MediaUrl,
                    Timestamp = comment.CreatedAt,
                    PostAuthorId = post?.AuthorId ?? string.Empty,
                    IsEdited = false
                });

                return new ResponseWrapper<CommentResponse>
                {
                    Data = ToDto(comment),
                    Message = "Comment created successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<CommentResponse>
                {
                    Message = "Failed to create comment",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<CommentResponse>> UpdateCommentAsync(EditCommentRequest dto)
        {
            try
            {
                // Validate required fields for full update
                if (string.IsNullOrEmpty(dto.CommentId) || 
                    string.IsNullOrEmpty(dto.Content) || 
                    string.IsNullOrEmpty(dto.UserId))
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "Validation error",
                        ErrorType = ErrorType.Validation,
                        Errors = new List<string> 
                        { 
                            "Comment ID, Content, and User ID are required for update" 
                        }
                    };
                }

                // Validate media if present
                if (dto.HasMedia && dto.Media != null && dto.MediaType == MediaType.UNKNOWN)
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "Invalid media type",
                        ErrorType = ErrorType.Validation,
                        Errors = new List<string> { "A valid media type must be specified when including media" }
                    };
                }

                var comment = await _commentRepository.GetByIdAsync(dto.CommentId);
                if (comment == null)
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "Comment not found",
                        ErrorType = ErrorType.NotFound,
                        Errors = new List<string> { "Comment not found" }
                    };
                }

                if (comment.AuthorId != dto.UserId)
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "Unauthorized",
                        ErrorType = ErrorType.UnAuthorized,
                        Errors = new List<string> { "You can only edit your own comments" }
                    };
                }

                // Validate content length
                var contentValidation = ValidateContent(dto.Content);
                if (contentValidation != null)
                {
                    return contentValidation;
                }
                // Update content (required in full update)
                comment.Content = dto.Content;
                comment.IsEdited = true;

                // Handle media updates for full update
                if (dto.HasMedia)
                {
                    try
                    {
                        // If new media is provided, update it
                        if (dto.Media != null)
                        {
                            // If there was existing media, update it
                            if (!string.IsNullOrEmpty(comment.MediaUrl))
                            {
                                var mediaUrls = new List<string> { comment.MediaUrl };
                                var editRequest = new MediaUploadRequestDto
                                {
                                    File = dto.Media,
                                    usageCategory = Service.Enums.UsageCategory.Comment,
                                    MediaType = dto.MediaType
                                };

                                var mediaResponse = await _mediaServiceClient.EditMediaAsync(editRequest, mediaUrls);
                                if (mediaResponse == null || !mediaResponse.Success || mediaResponse.Urls == null || !mediaResponse.Urls.Any())
                                {
                                    return new ResponseWrapper<CommentResponse>
                                    {
                                        Message = "Failed to update media",
                                        ErrorType = ErrorType.InternalServerError,
                                        Errors = new List<string> { "Failed to update media file" }
                                    };
                                }
                                comment.MediaUrl = mediaResponse.Urls.First();
                            }
                            else
                            {
                                // Add new media
                                var mediaRequest = new MediaUploadRequestDto
                                {
                                    File = dto.Media,
                                    usageCategory = Service.Enums.UsageCategory.Comment,
                                    MediaType = dto.MediaType
                                };

                                var mediaResponse = await _mediaServiceClient.UploadMediaAsync(mediaRequest);
                                if (mediaResponse == null || !mediaResponse.Success || mediaResponse.Urls == null || !mediaResponse.Urls.Any())
                                {
                                    return new ResponseWrapper<CommentResponse>
                                    {
                                        Message = "Failed to upload media",
                                        ErrorType = ErrorType.InternalServerError,
                                        Errors = new List<string> { "Failed to upload media file" }
                                    };
                                }
                                comment.MediaUrl = mediaResponse.Urls.First();
                            }
                        }
                        // If keeping existing media URL and it matches the provided one
                        else if (!string.IsNullOrEmpty(dto.MediaUrl) && dto.MediaUrl == comment.MediaUrl)
                        {
                            // Media URL is the same, no change needed
                            // The URL is already set, no need to modify it
                        }
                        // If no new media but HasMedia is true and no MediaUrl provided, it's an invalid state
                        else if (string.IsNullOrEmpty(dto.MediaUrl))
                        {
                            return new ResponseWrapper<CommentResponse>
                            {
                                Message = "Media URL is required when HasMedia is true",
                                ErrorType = ErrorType.Validation,
                                Errors = new List<string> { "Media URL must be provided when HasMedia is true" }
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        return new ResponseWrapper<CommentResponse>
                        {
                            Message = "Failed to process media",
                            ErrorType = ErrorType.InternalServerError,
                            Errors = new List<string> { $"An error occurred while processing media: {ex.Message}" }
                        };
                    }
                }
                // If media is being removed (HasMedia is false)
                else if (!dto.HasMedia && !string.IsNullOrEmpty(comment.MediaUrl))
                {
                    // Only delete media if MediaUrl is explicitly set to empty string
                    // or if MediaUrl matches the current media URL
                    if (dto.MediaUrl == string.Empty || dto.MediaUrl == comment.MediaUrl)
                    {
                        try
                        {
                            // Delete the existing media
                            var deleted = await _mediaServiceClient.DeleteMediaAsync(new List<string> { comment.MediaUrl });
                            if (!deleted)
                            {
                                Console.WriteLine($"Warning: Failed to delete media for comment {comment.Id}");
                            }
                            comment.MediaUrl = "";
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting media for comment {comment.Id}: {ex.Message}");
                            comment.MediaUrl = "";
                        }
                    }
                    // If MediaUrl was provided and doesn't match, don't delete - it might be a stale client
                    // or the client wants to keep the existing media
                }

                // Update the comment
                comment.IsEdited = true;
                await _commentRepository.UpdateAsync(comment);

                var post = await _postRepository.GetPostByIdAsync(comment.PostId);
                if (post == null || post.IsDeleted)
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "Post has been deleted or doesn't exist",
                        ErrorType = ErrorType.NotFound,
                        Errors = new List<string> { "Post has been deleted or doesn't exist" }
                    };
                }
                await _commentPublisher.PublishAsync(new CommentEvent
                {
                    EventType = EventType.Update,
                    CommentId = comment.Id.ToString(),
                    PostId = comment.PostId,
                    CommentAuthorId = comment.AuthorId,
                    Content = comment.Content,
                    MediaUrl = comment.MediaUrl,
                    Timestamp = comment.CreatedAt,
                    PostAuthorId = post?.AuthorId ?? string.Empty,
                    IsEdited = true
                });

                return new ResponseWrapper<CommentResponse>
                {
                    Data = ToDto(comment),
                    Message = "Comment updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<CommentResponse>
                {
                    Message = "Failed to update comment",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<bool>> DeleteCommentAsync(string commentId, string userId)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment is null)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "Comment not found",
                        ErrorType = ErrorType.NotFound,
                        Errors = new List<string> { "Comment not found" }
                    };
                }

                if (comment.AuthorId != userId)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "Unauthorized",
                        ErrorType = ErrorType.UnAuthorized,
                        Errors = new List<string> { "You can only delete your own comments" }
                    };
                }

                var post = await _postRepository.GetPostByIdAsync(comment.PostId);
                if (post == null || post.IsDeleted)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "Post has been deleted or doesn't exist",
                        ErrorType = ErrorType.NotFound,
                        Errors = new List<string> { "Post has been deleted or doesn't exist" }
                    };
                }
                // Privacy filtration for delete
                if (post.Privacy == Privacy.OnlyMe && post.AuthorId != userId)
                {
                    return new ResponseWrapper<bool>
                    {
                        Data = false,
                        Message = "You are not allowed to delete a comment on this post",
                        ErrorType = ErrorType.UnAuthorized,
                        Errors = new List<string> { "You are not allowed to delete a comment on this post" }
                    };
                }
                await _commentRepository.DeleteAsync(commentId);

                await _commentPublisher.PublishAsync(new CommentEvent
                {
                    EventType = EventType.Delete,
                    CommentId = comment.Id.ToString(),
                    PostId = comment.PostId,
                    CommentAuthorId = comment.AuthorId,
                    PostAuthorId = post.AuthorId
                });

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "Comment deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Failed to delete comment",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        public async Task<ResponseWrapper<CommentResponse>> GetCommentAsync(string commentId)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(commentId);
                if (comment == null)
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "Comment not found",
                        ErrorType = ErrorType.NotFound,
                        Errors = new List<string> { "Comment not found" }
                    };
                }

                return new ResponseWrapper<CommentResponse>
                {
                    Data = ToDto(comment),
                    Message = "Comment retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseWrapper<CommentResponse>
                {
                    Message = "Failed to retrieve comment",
                    Errors = new List<string> { ex.Message },
                    ErrorType = ErrorType.InternalServerError
                };
            }
        }

        private CommentResponse ToDto(Comment c) => new()
        {
            CommentId = c.Id.ToString(),
            PostId = c.PostId,
            AuthorId = c.AuthorId,
            CommentContent = c.Content ?? "",
            MediaUrl = c.MediaUrl,
            CreatedAt = c.CreatedAt,
            IsEdited = c.IsEdited,
            ReactionsCount = c.ReactCount
        };
    }
}
