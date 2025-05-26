using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    internal class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } =  string.Empty;

        public string AuthorId { get; set; } = string.Empty;

        public string PostId { get; set; } = string.Empty;

        public string CommentId { get; set; } = string.Empty;

    }
}
