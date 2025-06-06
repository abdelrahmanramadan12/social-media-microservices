using Application.DTOs;
using Application.DTOs.Responses;

namespace Application.Services
{
    public class MappingResult<T>
    {
        public T Item { get; set; }
        public bool Success => Item != null && !Errors.Any();
        public List<string> Errors { get; set; } = new();
        public ErrorType ErrorType { get; set; } = ErrorType.None;
    }
}
