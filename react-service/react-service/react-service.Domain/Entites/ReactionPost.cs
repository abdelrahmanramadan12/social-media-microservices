using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
namespace react_service.Domain.Entites
{
    

   
        public class ReactionPost
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            [BsonElement("postId")]
            public string PostId { get; set; }

            [BsonElement("userId")]
            public string UserId { get; set; }

            [BsonElement("reactionType")]
            public string ReactionType { get; set; }

            [BsonElement("postCreatedTime")]
            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            public DateTime PostCreatedTime { get; set; }
        }
   

}
