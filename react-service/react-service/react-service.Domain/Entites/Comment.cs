using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Domain.Entites
{
    public class Comment
    {
        [BsonId]
        public string CommentId { get; set; }
        public string AuthorId { get; set; }
        [BsonElement("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}
