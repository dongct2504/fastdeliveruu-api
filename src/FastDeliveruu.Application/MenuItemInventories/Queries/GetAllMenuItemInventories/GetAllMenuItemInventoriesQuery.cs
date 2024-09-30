using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using MediatR;

namespace FastDeliveruu.Application.MenuItemInventories.Queries.GetAllMenuItemInventories;

public class GetAllMenuItemInventoriesQuery : IRequest<PagedList<MenuItemInventoryDto>>
{
    public GetAllMenuItemInventoriesQuery(DefaultParams defaultParams)
    {
        DefaultParams = defaultParams;
    }

    public DefaultParams DefaultParams { get; }
}
