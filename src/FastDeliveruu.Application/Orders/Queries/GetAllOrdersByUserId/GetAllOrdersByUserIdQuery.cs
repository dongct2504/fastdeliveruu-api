using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.OrderDtos;
using MediatR;

namespace FastDeliveruu.Application.Orders.Queries.GetAllOrdersByUserId;

public class GetAllOrdersByUserIdQuery : IRequest<PaginationResponse<OrderDto>>
{
    public GetAllOrdersByUserIdQuery(Guid userId, int pageNumber)
    {
        UserId = userId;
        PageNumber = pageNumber;
    }

    public Guid UserId { get; }

    public int PageNumber { get; }
}
