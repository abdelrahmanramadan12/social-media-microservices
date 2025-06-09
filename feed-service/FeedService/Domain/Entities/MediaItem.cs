namespace Domain.Entities
{
    public enum MediaType
    {
        Picture,
        Video
    }

    public class MediaItem
    {
        public MediaType MediaType { get; set; }
        public string MediaURL { get; set; }
        public string ThumbnailURL { get; set; }
    }
}
