using FastDeliveruu.Application.Dtos.OrderDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<Result<OrderHeaderDetailDto>>
{
    public GetOrderByIdQuery(Guid userId, Guid id)
    {
        UserId = userId;
        Id = id;
    }

    public Guid UserId { get; }

    public Guid Id { get; }
}
