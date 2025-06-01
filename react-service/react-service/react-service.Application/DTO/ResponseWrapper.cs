using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.DTO
{
    public class ResponseWrapper<T>
    {
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }
}
