namespace Application.DTOs.Reactions
{
    public class GetReactsOfPostRequest
    {
        public required string PostId { get; set; }
        public string? Next { get; set; }
        public string? UserId { get; set; }
    }
}
