namespace Domain.CacheEntities.Comments
{
    public class CachedCommentsNotification
    {
        public string UserId { get; set; } = string.Empty; // Unique identifier for the user who will receive the notification
        public List<CommnetNotificationDetails> CommnetDetails { get; set; } = []; // Details of the comment, including comment ID and content
    }
}