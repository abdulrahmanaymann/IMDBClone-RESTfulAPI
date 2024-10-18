namespace IMDbClone.Core.Utilities
{
    public class PaginatedResult<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int TotalCount { get; set; }
    }
}
