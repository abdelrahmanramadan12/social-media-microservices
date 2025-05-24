using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public enum ErrorType
    {
        None,
        NotFound, 
        BadRequest,
        UnAuthorized,
        Validation,
        InternalServerError
    }
    public class ServiceResponse<T>
    {
        public T DataItem { get; set; }
        public List<T> DataList { get; set; }
        public bool IsValid => Errors.Count > 0;
        public ErrorType ErrorType { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
