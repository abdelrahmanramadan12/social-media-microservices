using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MappingResult <T>
    {
        public T Item {  get; set; } 
        public bool Success => Item != null && Errors.Count() > 0;
        public List<string> Errors { get; set; }
        public ErrorType ErrorType { get; set; }
    }
}
