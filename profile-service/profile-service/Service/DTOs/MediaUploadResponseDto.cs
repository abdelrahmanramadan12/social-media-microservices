namespace Service.DTOs
{
    public class MediaUploadResponseDto
    {
        public bool Success { get; set; }
        public int Uploaded { get; set; }
        public List<string> Urls { get; set; }
    }
}
