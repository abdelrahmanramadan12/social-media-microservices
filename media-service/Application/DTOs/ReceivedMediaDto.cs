using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ReceivedMediaDto
    {
        public IEnumerable<IFormFile> files = null!;
        public UsageCategory usageCategory;
        public MediaType mediaType;
    }
}
