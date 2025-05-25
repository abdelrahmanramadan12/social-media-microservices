using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserNotification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string SourceUserId { get; set; }
        public string DestinationUserId { get; set; }
        public bool Read { get; set; }

        [BsonElement("notification_reason_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string NotificationReasonId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string EntityId { get; set; }
    }

}
