namespace Domain.CacheEntities.Message
{
    public class CachedMessage
    {
        public string RecieverUserId { get; set; } = string.Empty; // ID of the user who sent the message
        public List<MessageInfo> Messages { get; set; } = []; // List of messages sent by the user
    }

    public class MessageInfo
    {
        public UserSkeleton User { get; set; } = null!; // User who sent the message
        public string MessageId { get; set; } = string.Empty; // Unique identifier for the message

    }
}
