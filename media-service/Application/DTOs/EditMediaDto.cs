using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class EditMediaDto
    {
        public string ImageUrl = string.Empty;
        public string filePath = string.Empty;
        public UsageCategory usageCategory;
        public string? folder = null;
    }
}
