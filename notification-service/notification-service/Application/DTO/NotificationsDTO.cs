using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.DTO
{
    public class NotificationsDTO
    {
        public string SourceUserImageUrl { get; set; } = string.Empty;
        public string SourceUsername { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty; // ID of the entity related to the notification (e.g., message, post, etc.)
        public NotificationEntity EntityName { get; set; }
        public string? NotificationPreview { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedTime { get; set; }

    }
}