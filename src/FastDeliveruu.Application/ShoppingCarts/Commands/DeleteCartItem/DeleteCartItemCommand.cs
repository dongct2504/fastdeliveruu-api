using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.DeleteCartItem;

public class DeleteCartItemCommand : IRequest<Result<int>>
{
    public DeleteCartItemCommand(Guid userId, Guid menuItemId)
    {
        UserId = userId;
        MenuItemId = menuItemId;
    }

    public Guid UserId { get; }

    public Guid MenuItemId { get; }
}
