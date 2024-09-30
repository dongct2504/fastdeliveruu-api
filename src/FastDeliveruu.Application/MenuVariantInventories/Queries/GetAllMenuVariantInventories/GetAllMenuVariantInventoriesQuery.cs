using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using MediatR;

namespace FastDeliveruu.Application.MenuVariantInventories.Queries.GetAllMenuVariantInventories;

public class GetAllMenuVariantInventoriesQuery : IRequest<PagedList<MenuVariantInventoryDto>>
{
    public GetAllMenuVariantInventoriesQuery(DefaultParams defaultParams)
    {
        DefaultParams = defaultParams;
    }

    public DefaultParams DefaultParams { get; }
}
