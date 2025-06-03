using Domain.CacheEntities;
using Domain.Enums;

namespace Domain.Events
{
    public enum ReactedEntity
    {
        Post,
        Comment,
        Message
    }
    public class ReactionEvent
    {
        public string Id { get; set; } = string.Empty; // Unique identifier for the reaction event, typically a GUID or ObjectId
        public string? ReactionEntityId { get; set; } = string.Empty; // Unique ID for the reaction event
        public string? AuthorEntityId { get; set; }
        public UserSkeleton User { get; set; } = null!; // User who reacted, using UserSkeleton for simplicity
        public ReactionType Type { get; set; }
        public ReactedEntity ReactedOn { get; set; } // Indicates what was reacted on (Post, Comment, Message)

        //public string? PostId { get; set; } = string.Empty; // Post ID if reacted on a post
        //public string? CommentId { get; set; } = string.Empty; // Comment ID if reacted on a comment
        //public string? MessageId { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;


    }
}
