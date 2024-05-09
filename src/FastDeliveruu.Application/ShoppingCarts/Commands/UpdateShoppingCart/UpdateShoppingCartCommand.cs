using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.UpdateShoppingCart;

public class UpdateShoppingCartCommand : IRequest<Result>
{
    public Guid LocalUserId { get; set; }

    public Guid MenuItemId { get; set; }

    public int Quantity { get; set; }
}
