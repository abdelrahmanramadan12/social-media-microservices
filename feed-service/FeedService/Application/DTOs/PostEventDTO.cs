using Domain.Entities;
using MongoDB.Bson;

namespace Application.DTOs
{
    public class PostEventDTO
    {
        public EventType EventType { get; set; }
        public ObjectId Id { get; set; }
        public string Content { get; set; }
        public ICollection<MediaItem> MediaList { get; set; }
        public bool IsEdited { get; set; }
        public DateTime Timestamp { get; set; }
        public Privacy Privacy { get; set; }
        public string AuthorId { get; set; }
    }
}
