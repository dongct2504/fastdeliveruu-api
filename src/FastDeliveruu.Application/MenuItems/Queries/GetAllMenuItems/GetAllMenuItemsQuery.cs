using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using MediatR;

namespace FastDeliveruu.Application.MenuItems.Queries.GetAllMenuItems;

public class GetAllMenuItemsQuery : IRequest<PagedList<MenuItemDto>>
{
    public GetAllMenuItemsQuery(MenuItemParams menuItemParams)
    {
        MenuItemParams = menuItemParams;
    }

    public MenuItemParams MenuItemParams { get; }
}
