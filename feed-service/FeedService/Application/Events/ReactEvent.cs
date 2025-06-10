using MongoDB.Bson;

namespace Application.Events
{
    public enum ReactEventType
    {
        Like,
        Unlike
    }

    public class ReactEvent : QueueEvent
    {
        public ReactEventType EventType { get; set; }
        public string UserId { get; set; }
        public string PostId { get; set; }
        public string PostAuthorId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
