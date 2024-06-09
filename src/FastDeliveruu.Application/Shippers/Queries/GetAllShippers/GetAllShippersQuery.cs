using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using MediatR;

namespace FastDeliveruu.Application.Shippers.Queries.GetAllShippers;

public class GetAllShippersQuery : IRequest<PagedList<ShipperDto>>
{
    public GetAllShippersQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber { get; }

    public int PageSize { get; }
}