using Application.DTOs;

namespace Application.Services
{
    public class MappingResult<T>
    {
        public T Item { get; set; }
        public bool Success => Item != null && Errors.Count() > 0;
        public List<string> Errors { get; set; }
        public ErrorType ErrorType { get; set; }
    }
}
