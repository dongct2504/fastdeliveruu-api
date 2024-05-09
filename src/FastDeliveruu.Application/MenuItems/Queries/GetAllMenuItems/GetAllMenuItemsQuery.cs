using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using MediatR;

namespace FastDeliveruu.Application.MenuItems.Queries.GetAllMenuItems;

public class GetAllMenuItemsQuery : IRequest<PaginationResponse<MenuItemDetailDto>>
{
    public GetAllMenuItemsQuery(Guid? genreId, Guid? restaurantId, int pageNumber)
    {
        GenreId = genreId;
        RestaurantId = restaurantId;
        PageNumber = pageNumber;
    }

    public Guid? GenreId { get; }

    public Guid? RestaurantId { get; }

    public int PageNumber { get; }
}
