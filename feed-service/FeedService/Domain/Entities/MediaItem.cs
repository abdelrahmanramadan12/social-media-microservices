using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public enum MediaType
    {
        Picture,
        Video
    }

    public class MediaItem
    {
        [BsonElement("type")]
        public MediaType Type { get; set; }
        [BsonElement("url")]
        public string Url { get; set; }
        [BsonElement("thumbnailUrl")]
        public string ThumbnailURL { get; set; }
    }
}
