using Domain.Entities;
using Domain.Enums;
using Domain.IRepository;
using MongoDB.Bson;
using Service.DTOs.Requests;
using Service.DTOs.Responses;
using Service.Events;
using Service.Interfaces.CommentServices;
using Service.Interfaces.MediaServices;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.CommentServices
{
    public class CommentService : ICommentService
    {
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
                if (string.IsNullOrWhiteSpace(dto.PostId))
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "PostId is required",
                        ErrorType = ErrorType.Validation,
                        Errors = new List<string> { "PostId is required" }
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

                var post = await _postRepository.GetPostByIdAsync(comment.PostId);

                await _commentPublisher.PublishAsync(new CommentEvent
                {
                    EventType = CommentEventType.Created,
                    Data = new CommentData
                    {
                        CommentId = comment.Id.ToString(),
                        PostId = comment.PostId,
                        CommentAuthorId = comment.AuthorId,
                        Content = comment.Content,
                        MediaUrl = comment.MediaUrl,
                        CreatedAt = comment.CreatedAt,
                        PostAuthorId = post?.AuthorId ?? string.Empty,
                        IsEdited = false
                    }
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
                // 1. Validate required fields
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

                // 2. Validate media type if media file is present
                if (dto.HasMedia && dto.Media != null && dto.MediaType == MediaType.UNKNOWN)
                {
                    return new ResponseWrapper<CommentResponse>
                    {
                        Message = "Invalid media type",
                        ErrorType = ErrorType.Validation,
                        Errors = new List<string> { "A valid media type must be specified when including media" }
                    };
                }

                // 3. Get and validate comment
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

                // 4. Update content
                comment.Content = dto.Content;
                comment.IsEdited = true;

                // 5. Handle media logic
                if (dto.HasMedia)
                {
                    try
                    {
                        if (dto.Media != null)
                        {
                            // Add or replace media
                            var mediaRequest = new MediaUploadRequestDto
                            {
                                File = dto.Media,
                                usageCategory = Service.Enums.UsageCategory.Comment,
                                MediaType = dto.MediaType
                            };

                            if (!string.IsNullOrEmpty(comment.MediaUrl))
                            {
                                // Update existing media
                                var mediaResponse = await _mediaServiceClient.EditMediaAsync(mediaRequest, new List<string> { comment.MediaUrl });
                                if (mediaResponse?.Success != true || mediaResponse.Urls == null || !mediaResponse.Urls.Any())
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
                                // Upload new media
                                var mediaResponse = await _mediaServiceClient.UploadMediaAsync(mediaRequest);
                                if (mediaResponse?.Success != true || mediaResponse.Urls == null || !mediaResponse.Urls.Any())
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
                        else if (!string.IsNullOrEmpty(dto.MediaUrl) && dto.MediaUrl == comment.MediaUrl)
                        {
                            // No change to media
                        }
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
                else // HasMedia == false
                {
                    if (string.IsNullOrEmpty(dto.MediaUrl))
                    {
                        // Remove existing media
                        if (!string.IsNullOrEmpty(comment.MediaUrl))
                        {
                            try
                            {
                                var deleted = await _mediaServiceClient.DeleteMediaAsync(new List<string> { comment.MediaUrl });
                                if (!deleted)
                                {
                                    Console.WriteLine($"Warning: Failed to delete media for comment {comment.Id}");
                                }
                                comment.MediaUrl = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error deleting media for comment {comment.Id}: {ex.Message}");
                                comment.MediaUrl = string.Empty;
                            }
                        }
                    }
                    else if (dto.MediaUrl == comment.MediaUrl)
                    {
                        // Media URL matches existing — retain it
                        // No action needed
                    }
                    else
                    {
                        // Invalid state: HasMedia is false but media URL differs
                        return new ResponseWrapper<CommentResponse>
                        {
                            Message = "Invalid media state",
                            ErrorType = ErrorType.Validation,
                            Errors = new List<string> { "HasMedia is false, but a different MediaUrl was provided" }
                        };
                    }
                }

                // 6. Save changes
                await _commentRepository.UpdateAsync(comment);

                // 7. Publish update event
                var post = await _postRepository.GetPostByIdAsync(comment.PostId);
                await _commentPublisher.PublishAsync(new CommentEvent
                {
                    EventType = CommentEventType.Updated,
                    Data = new CommentData
                    {
                        CommentId = comment.Id.ToString(),
                        PostId = comment.PostId,
                        CommentAuthorId = comment.AuthorId,
                        Content = comment.Content,
                        MediaUrl = comment.MediaUrl,
                        CreatedAt = comment.CreatedAt,
                        PostAuthorId = post?.AuthorId ?? string.Empty,
                        IsEdited = true
                    }
                });

                // 8. Return success response
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

                await _commentRepository.DeleteAsync(commentId);

                await _commentPublisher.PublishAsync(new CommentEvent
                {
                    EventType = CommentEventType.Deleted,
                    Data = new CommentData
                    {
                        CommentId = comment.Id.ToString(),
                        PostId = comment.PostId,
                        CommentAuthorId = comment.AuthorId,
                        PostAuthorId = (await _postRepository.GetPostByIdAsync(comment.PostId))?.AuthorId ?? string.Empty
                    }
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
            IsEdited = c.IsEdited
        };
    }
}
