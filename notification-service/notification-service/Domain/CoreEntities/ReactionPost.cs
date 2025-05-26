//using MongoDB.Bson.Serialization.Attributes;
//using MongoDB.Bson;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Domain.CoreEntities
//{
//    public class ReactionPost
//    {
//        [BsonId]
//        [BsonRepresentation(BsonType.ObjectId)]
//        public string Id { get; set; } = string.Empty; // Unique identifier for the reaction

//        public string MyId { get; set; } = string.Empty;

//        public List<string> UsersId { get; set; } = []; // Unique identifier for the user who reacted

//        public List<string> PostId { get; set; } = [];

//    }
//}
