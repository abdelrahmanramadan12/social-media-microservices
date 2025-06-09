namespace Application.DTOs
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public T? Value { get; set; }
        public List<string> Errors { get; set; }
    }
}
