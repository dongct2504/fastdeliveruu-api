using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Queries.GetAllShoppingCarts;

public class GetAllShoppingCartsByUserIdQuery : IRequest<PagedList<ShoppingCartDto>>
{
    public GetAllShoppingCartsByUserIdQuery(Guid userId, int pageNumber)
    {
        UserId = userId;
        PageNumber = pageNumber;
    }

    public Guid UserId { get; }

    public int PageNumber { get; }
}
