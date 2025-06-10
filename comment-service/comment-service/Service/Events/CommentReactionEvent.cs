using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Events
{
    public enum ReactionEventType
    {
        Like,
        Unlike
    }
    public class CommentReactionEvent : QueueEvent
    {
        public string CommentId { get; set; } = default!;
        public ReactionEventType EventType { get; set; }
        public string ReactionAuthorId { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string PostAuthorId { get; set; } = default!;
    }
}
