using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Comment
    {
        [Key]
        [BsonId]
        public ObjectId Id { get; set; }
        public string PostId { get; set; } = default!;
        public string AuthorId { get; set; } = default!;
        public string? Content { get; set; }
        public string? MediaUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ReactCount { get; set; }
        public bool IsEdited { get; set; }
    }
}
