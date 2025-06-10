namespace react_service.Application.Events
{
    public class CommentReactionEvent : QueueEvent
    {
        public string CommentId { get; set; }
        public string UserId { get; set; }
        public ReactionEventType EventType { get; set; }
    }
}
