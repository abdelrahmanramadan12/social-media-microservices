using react_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Events
{

        public enum ReactedEntity
        {
            Post,
            Comment,
            Message
        }
        public class ReactionEventNoti : QueueEvent
        {
            public string Id { get; set; } = string.Empty; // Unique identifier for the reaction event, typically a GUID or ObjectId
            public string? ReactionEntityId { get; set; } = string.Empty; // Unique ID for the reaction event
            public string? AuthorEntityId { get; set; }
            public UserSkeleton User { get; set; } = null!; // User who reacted, using UserSkeleton for simplicity
            public ReactionType Type { get; set; }
            public ReactedEntity ReactedOn { get; set; } // Indicates what was reacted on (Post, Comment, Message)
            public string? Content { get; set; } = string.Empty;


        }
        public class UserSkeleton
        {
            public string Id { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty; // Unique identifier for the user who reacted
            public bool Seen { get; set; }
            public DateTime CreatedAt { get; set; } // Timestamp of when the reaction was created

        }
   
}
