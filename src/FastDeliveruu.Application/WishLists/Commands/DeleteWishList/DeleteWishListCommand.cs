using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.WishLists.Commands.DeleteWishList;

public class DeleteWishListCommand : IRequest<Result>
{
    public DeleteWishListCommand(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
