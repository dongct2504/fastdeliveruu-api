namespace FastDeliveruu.Application.Dtos;

public class PagedList<T>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public int TotalPages => (PageSize + TotalRecords - 1) / PageSize;

    public bool HasNextPage => PageNumber * PageSize < TotalRecords;

    public bool HasPreviousPage => PageNumber > 1;

    public IEnumerable<T> Items { get; set; } = new List<T>();
}