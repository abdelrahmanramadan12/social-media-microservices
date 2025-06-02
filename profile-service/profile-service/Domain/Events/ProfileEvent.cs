namespace Domain.Events
{
    public enum ProfileEventType
    {
        ProfileAdded,
        ProfileUpdated,
        ProfileDeleted
    }

    public class ProfileEventData
    {
        public string UserId { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }

    public class ProfileEvent
    {
        public ProfileEventType EventType { get; set; }
        public ProfileEventData? User { get; set; }
    }
}
