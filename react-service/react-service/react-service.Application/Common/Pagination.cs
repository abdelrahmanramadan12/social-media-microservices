using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace react_service.Application.Common
{
    public class Pagination<T>
    {

        public List<T> Reactions { get; set; } = new();
        public int? NextCursor { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }

        public Pagination(List<T> items, int pageSize, int? nextCursor, bool hasMore)
        {
            Reactions = items;
            PageSize = pageSize;
            NextCursor = nextCursor;
            HasMore = hasMore;
        }
    }
}
