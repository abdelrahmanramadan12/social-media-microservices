namespace Application.DTOs
{
    public class MediaUploadResponse
    {
        public bool Success { get; set; }
        public int Uploaded { get; set; }
        public List<string> Urls { get; set; }
    }
}
