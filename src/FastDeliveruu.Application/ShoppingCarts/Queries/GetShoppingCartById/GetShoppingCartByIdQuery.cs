using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Queries.GetShoppingCartById;

public class GetShoppingCartByIdQuery : IRequest<Result<ShoppingCartDto>>
{
    public GetShoppingCartByIdQuery(Guid localUserId, Guid menuItemId)
    {
        LocalUserId = localUserId;
        MenuItemId = menuItemId;
    }

    public Guid LocalUserId { get; }

    public Guid MenuItemId { get; }
}
