using System.Collections.Generic;

namespace Application.DTOs
{
    public class ServiceResponse<T>
    {
        public bool IsValid { get; set; }
        public T DataItem { get; set; }
        public List<T> DataList { get; set; }
        public List<string> Errors { get; set; }
        public string NextCursor { get; set; }
        public ErrorType ErrorType { get; set; }
    }

    public enum ErrorType
    {
        NotFound,
        BadRequest,
        UnAuthorized,
        InternalServerError
    }
} 