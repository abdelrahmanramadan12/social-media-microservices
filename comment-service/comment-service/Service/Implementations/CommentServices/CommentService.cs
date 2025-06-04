using Domain.DTOs;
using Domain.Entities;
using Domain.Events;
using Domain.IRepository;
using MongoDB.Bson;
using Service.DTOs.Requests;
using Service.DTOs.Responses;
using Service.Interfaces.CommentServices;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.CommentServices
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentDeletedPublisher _commentDeletedPublisher;
        private readonly ICommentCreatedPublisher _commentCreatedPublisher;

        public CommentService(ICommentRepository commentRepository, ICommentDeletedPublisher commentDeletedPublisher, ICommentCreatedPublisher commentCreatedPublisher)
        {
            _commentRepository = commentRepository;
            _commentDeletedPublisher = commentDeletedPublisher;
            _commentCreatedPublisher = commentCreatedPublisher;
        }

        public async Task<ResponseWrapper<PagedCommentsResponse>> ListCommentsAsync(string postId, string? nextCommentId = null)
        {
            try
            {
                var PageSize = 10;
                string? decryptedCursor = null;
                if (!string.IsNullOrWhiteSpace(nextCommentId))
                {
                    try
                    {
                        decryptedCursor = nextCommentId;
                    }
                    catch
                    {
                        decryptedCursor = null;
                    }
                }

                var comments = (await _commentRepository
                                    .GetByPostIdCursorAsync(postId, decryptedCursor, PageSize))
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

                var comment = new Comment
                {
                    Id = ObjectId.GenerateNewId(),
                    PostId = dto.PostId,
                    AuthorId = dto.UserId,
                    Content = dto.Content,
                    CreatedAt = DateTime.Now,
                    IsEdited = false,
                    ReactCount = 0
                };

                await _commentRepository.CreateAsync(comment);

                var post = await _postRepository.GetPostByIdAsync(comment.PostId);

                await _commentCreatedPublisher.PublishAsync(new CommentCreatedEvent
                {
                    PostId = comment.PostId,
                    Content = comment.Content,
                    MediaURL = comment?.MediaUrl,
                    CommentAuthorId = comment.AuthorId,
                    CreatedAt = comment.CreatedAt,
                    PostAuthorId = post.AuthorId,
                    IsEdited = false
                });

                return new ResponseWrapper<CommentResponse>
                {
                    Data = ToDto(comment),
                    Message = "Comment created successfully",
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

                comment.Content = dto.Content;
                comment.IsEdited = true;

                if (dto.Media != null)
                {
                    //comment.MediaUrl = Url returns from media service
                }

                await _commentRepository.UpdateAsync(comment);
                
                return new ResponseWrapper<CommentResponse>
                {
                    Data = ToDto(comment),
                    Message = "Comment updated successfully",
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

                await _commentDeletedPublisher.PublishAsync(new CommentDeletedEvent
                {
                    PostId = comment.PostId,
                });

                return new ResponseWrapper<bool>
                {
                    Data = true,
                    Message = "Comment deleted successfully",
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
