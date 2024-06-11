using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.DeleteCustomerCart;

public class DeleteCustomerCartCommand : IRequest<Result>
{
    public DeleteCustomerCartCommand(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
