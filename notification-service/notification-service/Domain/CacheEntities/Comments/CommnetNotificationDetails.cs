namespace Domain.CacheEntities.Comments
{
    public class CommnetNotificationDetails
    {
        public UserSkeleton User { get; set; } = null!; // User data for the reaction, including user ID, profile image URL, and username
        public string PostId { get; set; } = string.Empty; // Unique identifier for the post the comment is associated with
        public string CommentId { get; set; } = string.Empty; // Unique identifier for the comment
        public string Content { get; set; } = string.Empty; // Content of the comment
    }
}
