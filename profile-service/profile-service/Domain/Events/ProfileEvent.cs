using Domain.DTOs;

namespace Domain.Events
{
    public enum ProfileEventType
    {
        ProfileAdded,
        ProfileUpdated,
        ProfileDeleted
    }
    public class ProfileEvent
    {
        public ProfileEventType EventType { get; set; }
        public SimpleUserDto? User { get; set; }
    }
}
