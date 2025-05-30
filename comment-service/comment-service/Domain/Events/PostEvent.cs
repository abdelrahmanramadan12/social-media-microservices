namespace Domain.Events
{
    public enum postEventType
    {
        PostAdded,
        PostDeleted
    }
    public class PostEvent
    {
        public postEventType EventType { get; set; }
        public PostData? Data { get; set; }
    }
}
