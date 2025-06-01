namespace Application.DTOs.Reaction
{
    public class ResponseWrapper<T>
    {
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public bool Success => Errors == null || Errors.Count == 0;
    }
} 