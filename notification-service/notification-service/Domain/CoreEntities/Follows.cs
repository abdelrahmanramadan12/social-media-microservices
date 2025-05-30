using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CoreEntities
{
    public class Follows
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty; // Unique identifier for the follow relationship

        public string MyId { get; set; } = string.Empty;  

        public List<string> FollowersId { get; set; } = [];   // List of IDs that follow me

        //  Track notifs which Author read relate to follow entity 
        public List<string> FollowsNotifReadByAuthor { get; set; } = [];
    }
}
