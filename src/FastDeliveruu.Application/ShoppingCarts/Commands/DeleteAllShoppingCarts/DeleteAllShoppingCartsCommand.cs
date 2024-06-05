using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.DeleteAllShoppingCarts;

public class DeleteAllShoppingCartsCommand : IRequest<Result>
{
    public DeleteAllShoppingCartsCommand(Guid localUserId)
    {
        LocalUserId = localUserId;
    }

    public Guid LocalUserId { get; set; }
}
