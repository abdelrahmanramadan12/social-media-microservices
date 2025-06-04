namespace Service.DTOs.Requests
{
    public class GetPagedCommentRequest
    {
        public string PostId { get; set; } = default!;
        public string? Next { get; set; }
    }
} 