using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.WishLists.Commands.DeleteWishListItem;

public class DeleteWishListItemCommand : IRequest<Result>
{
    public DeleteWishListItemCommand(Guid userId, Guid id)
    {
        UserId = userId;
        Id = id;
    }

    public Guid UserId { get; }
    public Guid Id { get; }
}
