namespace Domain.Events
{
    public enum ProfileEventType
    {
        Create,
        Update,
        Delete
    }

    public class ProfileEvent
    {
        public ProfileEventType EventType { get; set; }
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfilePictureUrl { get; set; }

    }
}
