using Domain.Enums;

namespace Application.Events
{
    public class ReactionEvent : QueueEvent
    {
        public string PostId { get; set; } = default!;
        public string PostAuthorId { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public ReactionEventType ReactionType { get; set; }
    }
}