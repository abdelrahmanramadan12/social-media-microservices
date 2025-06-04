using Domain.Enums;

namespace Domain.Events
{

    public class CommentEvent
    {
        public CommentEventType EventType { get; set; }
        public CommentData? Data { get; set; }
    }
}
