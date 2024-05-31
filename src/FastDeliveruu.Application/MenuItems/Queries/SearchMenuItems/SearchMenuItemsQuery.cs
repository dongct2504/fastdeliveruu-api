using FastDeliveruu.Application.Dtos.MenuItemDtos;
using MediatR;

namespace FastDeliveruu.Application.MenuItems.Queries.SearchMenuItems;

public class SearchMenuItemsQuery : IRequest<IEnumerable<MenuItemDto>>
{
    public SearchMenuItemsQuery(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
