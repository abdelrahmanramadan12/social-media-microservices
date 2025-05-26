using MongoDB.Bson;

namespace Application.DTOs
{
    internal enum ReactEventType
    {
        Like,
        Unlike
    }

    internal class ReactEventDTO
    {
        public ReactEventType EventType { get; set; }
        public string UserId { get; set; }
        public ObjectId PostId { get; set; }
        public string PostAuthorId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
