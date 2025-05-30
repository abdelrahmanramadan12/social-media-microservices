using System.Linq.Expressions;
using Domain.DTOs;
using Domain.Entities;
using Domain.Events;
using Domain.IRepository;
using MongoDB.Bson;
using Service.Interfaces.CommentServices;
using Service.Interfaces.RabbitMQServices;

namespace Service.Implementations.CommentServices
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly ICommentPublisher _commentPublisher;

        public CommentService(ICommentRepository commentRepository, ICommentPublisher commentPublisher)
        {
            _commentRepository = commentRepository;
            _commentPublisher = commentPublisher;
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
                NextCommentIdHash = nextCursor
            };

            return dto;

        }


        public async Task<CommentResponseDto> CreateCommentAsync(CreateCommentRequestDto dto)
        {
            ///====> call post/follow service to check the ability to add comment in this post  --> based on privacy
            /// if allowed

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

            await _commentRepository.CreateAsync(comment);

            var post = await _postRepository.GetPostByIdAsync(comment.PostId);

            // Notify the post service about the new comment
            await _commentPublisher.PublishAsync(new CommentEvent
            {
                EventType = CommentEventType.Created,
                Data = new CommentData
                {

                    CommentId = comment.Id.ToString(),
                    PostId = comment.PostId,
                    CommentAuthorId = comment.AuthorId,
                    Content = comment.Content ?? "",
                    CreatedAt = comment.CreatedAt,
                    PostAuthorId= post?.AuthorId ?? string.Empty   
                }
            });

            return ToDto(comment);
        }

        public async Task<CommentResponseDto?> UpdateCommentAsync(EditCommentRequestDto dto)
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
                //comment.MediaUrl = Url returns from media service
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


        //-----------------------------------------------------
        // Helper method to convert Comment to CommentResponseDto
        private CommentResponseDto ToDto(Comment c) => new()
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
