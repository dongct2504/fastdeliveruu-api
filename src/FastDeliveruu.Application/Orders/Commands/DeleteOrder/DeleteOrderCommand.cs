using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.Orders.Commands.DeleteOrder;

public class DeleteOrderCommand : IRequest<Result>
{
    public DeleteOrderCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
