namespace Application.DTOs
{
    public class ClientResponse<T>
    {
        public T? Data { get; set; }
        public List<string> Errors { get; set; }
        public string Message { get; set; }
    }
}
