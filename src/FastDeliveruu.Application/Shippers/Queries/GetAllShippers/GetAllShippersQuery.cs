using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using MediatR;

namespace FastDeliveruu.Application.Shippers.Queries.GetAllShippers;

public class GetAllShippersQuery : IRequest<PaginationResponse<ShipperDto>>
{
    public GetAllShippersQuery(int pageNumber)
    {
        PageNumber = pageNumber;
    }

    public int PageNumber { get; }
}