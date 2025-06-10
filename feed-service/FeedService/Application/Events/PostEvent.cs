using Domain.Entities;

namespace Application.Events
{
    public class PostEvent : QueueEvent
    {
        public EventType EventType { get; set; }
        public string PostId { get; set; }
        public string PostContent { get; set; }
        public List<MediaItem> Media { get; set; }
        public bool IsEdited { get; set; }
        public DateTime Timestamp { get; set; }
        public Privacy Privacy { get; set; }
        public string AuthorId { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
        public bool HasMedia { get; set; }
    }
}
