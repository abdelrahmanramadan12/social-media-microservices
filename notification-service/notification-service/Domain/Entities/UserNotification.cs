//using Domain.Enums;
//using MongoDB.Bson;
//using MongoDB.Bson.Serialization.Attributes;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Domain.Entities
//{
//    public class UserNotification
//    {
//        [BsonId]
//        [BsonRepresentation(BsonType.ObjectId)]
//        public string Id { get; set; } = string.Empty; // Unique identifier for the notification
//        public string SourceUserId { get; set; } = string.Empty;
//        public string DestinationUserId { get; set; } = string.Empty;
//        public bool Read { get; set; }
//        [BsonElement("notification_reason_id")]
//        [BsonRepresentation(BsonType.ObjectId)]
//        public NotificationEntity NotificationEntityId { get; set; }
//        public string EntityId { get; set; } = string.Empty;
//        public DateTime CreatedTime { get; set; }
//    }
//}
