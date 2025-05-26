using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    internal class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty; // Unique identifier for the post

        public string MyId { get; set; } = string.Empty;

        public List<string> PostId { get; set; } = []; // Unique identifier for the post, can be used for comments or replies

    }
}
