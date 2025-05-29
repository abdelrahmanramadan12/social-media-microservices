using Domain.Entities;
using MongoDB.Bson;

namespace Application.Events
{
    public class CommentEvent
    {
        public EventType EventType { get; set; }
        public ObjectId CommentId { get; set; }
        public string TextContent { get; set; }
        public MediaItem MediaItem { get; set; }
        public DateTime Timestamp { get; set; }
        public string AuthorId { get; set; }
        public ObjectId PostId { get; set; }
        public string PostAuthorId { get; set; }
    }
}
