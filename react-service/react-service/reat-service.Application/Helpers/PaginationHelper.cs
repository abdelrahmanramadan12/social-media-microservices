using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reat_service.Application.Helpers
{
    public static class PaginationHelper
    {
        public static string GenerateCursor(string objectId)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(objectId));
        }

        public static string DecodeCursor(string base64Cursor)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Cursor));
        }

    }
}
