namespace Domain.DTOs
{
    public class MediaUploadResponseDto
    {
        public bool Success { get; set; }
        public int Uploaded { get; set; }
        public string? Url { get; set; }
    }
}
