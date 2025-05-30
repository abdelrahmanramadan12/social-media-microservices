using Domain.Enums;

namespace Domain.Events
{
    public class PostEvent
    {
        public postEventType EventType { get; set; }
        public PostData? Data { get; set; }
    }
}
