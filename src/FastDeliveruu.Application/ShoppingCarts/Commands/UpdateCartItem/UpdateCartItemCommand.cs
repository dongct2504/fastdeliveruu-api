using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.UpdateCartItem;

public class UpdateCartItemCommand : IRequest<Result<int>>
{
    public Guid AppUserId { get; set; }

    public Guid MenuItemId { get; set; }

    public Guid? MenuVariantId { get; set; }

    public int Quantity { get; set; }
}
