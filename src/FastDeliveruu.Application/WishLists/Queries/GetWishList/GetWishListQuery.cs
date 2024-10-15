using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.WishListDtos;
using MediatR;

namespace FastDeliveruu.Application.WishLists.Queries.GetWishList;

public class GetWishListQuery : IRequest<PagedList<WishListDto>>
{
    public GetWishListQuery(WishListParams wishListParams)
    {
        WishListParams = wishListParams;
    }

    public WishListParams WishListParams { get; }
}
