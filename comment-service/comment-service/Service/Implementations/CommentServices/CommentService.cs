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
        private readonly ICommentDeletedPublisher _commentDeletedPublisher;
        private readonly ICommentCreatedPublisher _commentCreatedPublisher;

        public CommentService(ICommentRepository commentRepository, ICommentDeletedPublisher commentDeletedPublisher, ICommentCreatedPublisher commentCreatedPublisher)
        {
            _commentRepository = commentRepository;
            _commentDeletedPublisher = commentDeletedPublisher;
            _commentCreatedPublisher = commentCreatedPublisher;
        }

        public async Task<PagedCommentsDto> ListCommentsAsync(string postId, string? nextCommentIdHash = null)
        {
            var PageSize = 10;
            string? decryptedCursor = null;
            if (!string.IsNullOrWhiteSpace(nextCommentIdHash))
            {
                try
                {
                    //decryptedCursor = AesEncryptionService.Decrypt(nextCommentIdHash);
                    decryptedCursor = nextCommentIdHash;
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
            //: AesEncryptionService.Encrypt(comments.Last().Id.ToString());


            var dto = new PagedCommentsDto
            {
                Comments = comments.Select(ToDto),
                NextCommentIdHash = nextCursor
            };

            return dto;

        }


        public async Task<CommentResponseDto> CreateCommentAsync(CreateCommentRequestDto dto)
        {
            ///====> call post/follow service to check the ability to add comment in this post 
            /// if not allowed, throw an exception

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


            // Notify the post service about the new comment
            await _commentCreatedPublisher.PublishAsync(new CommentCreatedEvent
            {
                PostId = comment.PostId,
                Content = comment.Content,
                MediaURL = comment?.MediaUrl,
                CommentAuthorId = comment.AuthorId,
                CreatedAt = comment.CreatedAt,
                //PostAuthorId = dto.PostAuthorId,
                IsEdited = false
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
            await _commentDeletedPublisher.PublishAsync(new CommentDeletedEvent
            {
                PostId = comment.PostId,
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
