namespace Application.DTOs.Reaction
{
    public class GetReactsOfPostRequest
    {
        public string PostId { get; set; }
        public string? Next { get; set; }
    }
} 