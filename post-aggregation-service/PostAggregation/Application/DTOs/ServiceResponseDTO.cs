namespace Application.DTOs
{
    public class ServiceResponseDTO<T>
    {
        public bool Success { get; set; }
        public T Item { get; set; }
        public List<T> ItemList { get; set; }
        public List<string> Errors { get; set; }
    }
}
