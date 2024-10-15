using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.WishLists.Commands.UpdateWishListItem;

public class UpdateWishListItemCommand : IRequest<Result>
{
    public Guid Id { get; set; }

    public Guid AppUserId { get; set; }

    public Guid MenuItemId { get; set; }

    public Guid? MenuVariantId { get; set; }
}
