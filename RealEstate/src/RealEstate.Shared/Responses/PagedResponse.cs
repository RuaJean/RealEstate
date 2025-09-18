using System.Collections.Generic;

namespace RealEstate.Shared.Responses
{
    public class PagedResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public IReadOnlyList<T> Items { get; set; } = new List<T>();
    }
}


