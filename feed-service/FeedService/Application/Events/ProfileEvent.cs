namespace Application.Events
{
    public enum EventType
    {
        Create,
        Update,
        Delete
    }

    public class ProfileEvent : QueueEvent
    {
        public EventType EventType { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
