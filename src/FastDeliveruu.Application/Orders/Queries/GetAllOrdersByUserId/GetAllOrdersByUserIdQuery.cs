using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.OrderDtos;
using MediatR;

namespace FastDeliveruu.Application.Orders.Queries.GetAllOrdersByUserId;

public class GetAllOrdersByUserIdQuery : IRequest<PagedList<OrderDto>>
{
    public GetAllOrdersByUserIdQuery(Guid userId, int pageNumber, int pageSize)
    {
        UserId = userId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public Guid UserId { get; }

    public int PageNumber { get; }

    public int PageSize { get; }
}
