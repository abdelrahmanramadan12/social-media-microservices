namespace Application.DTOs.Responses
{
    public class PaginatedResponse<T> : ResponseWrapper<List<T>>
    {
        public string? Next { get; set; }
        public bool HasMore => !string.IsNullOrEmpty(Next);
    }
}
