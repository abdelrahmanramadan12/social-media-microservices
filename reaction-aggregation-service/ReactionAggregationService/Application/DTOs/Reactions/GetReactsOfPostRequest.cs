using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reactions
{
    public class GetReactsOfPostRequest
    {
        public required string PostId { get; set; }
        public string? Next { get; set; }

        public string GetQueryString()
        {
            var parameters = new List<string>();

            if (!string.IsNullOrEmpty(Next))
            {
                parameters.Add($"next={Uri.EscapeDataString(Next)}");
            }

            return string.Join("&", parameters);
        }
    }
}
