using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.IRepository;
using MongoDB.Bson;
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

        public CommentService(ICommentRepository commentRepository, ICommentPublisher commentPublisher ,IPostRepository postRepository,IMediaServiceClient mediaServiceClient)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _commentPublisher = commentPublisher;
            _mediaServiceClient = mediaServiceClient;
        }

        public async Task<PagedCommentsDto> ListCommentsAsync(string postId, string? nextCommentId = null)
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


            var dto = new PagedCommentsDto
            {
                Comments = comments.Select(ToDto),
                Next = nextCursor
            };

            return dto;

        }


        public async Task<CommentDto> CreateCommentAsync(CreateCommentRequestDto dto)
        {

            if (string.IsNullOrWhiteSpace(dto.PostId))
                throw new NullReferenceException("PostId is required.");

            var comment = new Comment
            {
                Id = ObjectId.GenerateNewId(),
                PostId = dto.PostId,
                AuthorId = dto.UserId,
                Content = dto.Content,
                CreatedAt = DateTime.Now,
                // MediaUrl = await ---> call media service via gateway 
                IsEdited = false,
                ReactCount = 0
            };

            if (dto.Media != null)
            {
                // Upload media to the media service and get the URL
                var mediaUploadResponse = await _mediaServiceClient.UploadMediaAsync(new MediaUploadRequestDto
                {
                    File = dto.Media,
                    MediaType = MediaType.IMAGE,
                    usageCategory = UsageCategory.Comment
                    
                });
                if (mediaUploadResponse.Success && mediaUploadResponse.Urls[0] != null )
                {
                    comment.MediaUrl = mediaUploadResponse.Urls[0];
                }
            }

            await _commentRepository.CreateAsync(comment);

            var post = await _postRepository.GetPostByIdAsync(comment.PostId);

            // Notify the post service about the new comment
            //await _commentPublisher.PublishAsync(new CommentEvent
            //{
            //    EventType = CommentEventType.Created,
            //    Data = new CommentData
            //    {

            //        CommentId = comment.Id.ToString(),
            //        PostId = comment.PostId,
            //        CommentAuthorId = comment.AuthorId,
            //        Content = comment.Content ?? "",
            //        CreatedAt = comment.CreatedAt,
            //        PostAuthorId = post?.AuthorId ?? string.Empty
            //    }
            //});

            return ToDto(comment);
        }

        public async Task<CommentDto?> UpdateCommentAsync(EditCommentRequestDto dto)
        {
            var comment = await _commentRepository.GetByIdAsync(dto.CommentId);
            if (comment == null)
                return null;

            if (comment.AuthorId != dto.UserId)
                throw new UnauthorizedAccessException("You can only edit your own comments.");

            comment.Content = dto.Content;
            comment.IsEdited = true;


            if (dto.Media != null)
            {
                // Upload new media to the media service and get the URL
                var mediaUploadResponse = await _mediaServiceClient.UploadMediaAsync(new MediaUploadRequestDto
                {
                    File = dto.Media,
                    MediaType = MediaType.IMAGE,
                    usageCategory = UsageCategory.Comment
                });

                // delete old media if exists
                // and assign the new media URL to the comment
                if (mediaUploadResponse.Success && mediaUploadResponse.Urls[0] != null)
                {
                    if (!string.IsNullOrWhiteSpace(comment.MediaUrl))
                    {
                        await _mediaServiceClient.DeleteMediaAsync(new[] { comment.MediaUrl });
                    }
                    comment.MediaUrl = mediaUploadResponse.Urls[0];

                }
            }

            await _commentRepository.UpdateAsync(comment);
            return ToDto(comment);
        }
        public async Task<bool> DeleteCommentAsync(string commentId, string userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment is null)
                return false;

            if (comment.AuthorId != userId)
                throw new UnauthorizedAccessException("You can only delete your own comments.");

            await _commentRepository.DeleteAsync(commentId);

            // If the comment has media, delete it from the media service
            if (!string.IsNullOrWhiteSpace(comment.MediaUrl))
            {
                await _mediaServiceClient.DeleteMediaAsync(new[] { comment.MediaUrl });
            }

            // Notify the post service about the deleted comment
            await _commentPublisher.PublishAsync(new CommentEvent
            {

                EventType = CommentEventType.Deleted,
                Data = new CommentData
                {
                    CommentId = comment.Id.ToString(),
                    PostId = comment.PostId,
                }

            });
            return true;
        }

        public async Task<CommentResponseDto?> GetCommentAsync(string commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                return new CommentResponseDto
                {
                    Data = null,
                    Messages = new List<string> { "Comment not found." },
                    Success = false
                };


            var dto = new CommentResponseDto
            {
                Success = true,
                Data = ToDto(comment)
            };
            return dto;
        }

        //-----------------------------------------------------
        // Helper method to convert Comment to CommentResponseDto
        private CommentDto ToDto(Comment c) => new()
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
