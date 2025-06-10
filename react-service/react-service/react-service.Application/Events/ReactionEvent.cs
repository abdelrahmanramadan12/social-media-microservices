using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Events
{
    public enum ReactionEventType
    {
        Like,
        Unlike
    }
    public class ReactionEvent : QueueEvent
    {
        public ReactionEventType EventType { get; set; }
        public string PostId { get; set; } = default!;
        public string PostAuthorId { get; set; } = default!;
        public string UserId { get; set; } = default!;
    }
}
