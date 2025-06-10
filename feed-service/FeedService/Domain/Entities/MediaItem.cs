namespace Domain.Entities
{
    public enum MediaType
    {
        Picture,
        Video
    }

    public class MediaItem
    {
        public MediaType Type { get; set; }
        public string Url { get; set; }
        public string ThumbnailURL { get; set; }
    }
}
