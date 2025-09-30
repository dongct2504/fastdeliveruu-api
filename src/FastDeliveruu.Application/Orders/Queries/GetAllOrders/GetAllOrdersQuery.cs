using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.OrderDtos;
using MediatR;

namespace FastDeliveruu.Application.Orders.Queries.GetAllOrders;

public class GetAllOrdersQuery : IRequest<PagedList<OrderDto>>
{
    public GetAllOrdersQuery(OrderParams orderParams)
    {
        OrderParams = orderParams;
    }

    public OrderParams OrderParams { get; }
}
