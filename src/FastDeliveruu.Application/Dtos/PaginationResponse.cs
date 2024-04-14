namespace FastDeliveruu.Application.Dtos;

public class PaginationResponse<T>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalRecords { get; set; }

    public int TotalPages => (PageSize + TotalRecords - 1) / PageSize;

    public IEnumerable<T>? Values { get; set; }
}