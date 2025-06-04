namespace Service.Events
{
    public class PostEvent
    {
        public postEventType EventType { get; set; }
        public PostData? Data { get; set; }
    }
}
