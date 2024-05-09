using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.DeleteShoppingCart;

public class DeleteShoppingCartCommand : IRequest<Result>
{
    public DeleteShoppingCartCommand(Guid localUserId, Guid menuItemId)
    {
        LocalUserId = localUserId;
        MenuItemId = menuItemId;
    }

    public Guid LocalUserId { get; }

    public Guid MenuItemId { get; }
}
