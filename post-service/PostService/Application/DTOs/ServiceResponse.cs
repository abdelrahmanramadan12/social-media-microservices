using Application.DTOs.Responses;

namespace Application.DTOs
{
    public class ServiceResponse<T>
    {
        public T DataItem { get; set; }
        public List<T> DataList { get; set; }
        public bool IsValid => !Errors.Any();
        public ErrorType ErrorType { get; set; } = ErrorType.None;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
