using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuVariantInventories.Queries.GetMenuVariantInventory;

public class GetMenuVariantInventoryQuery : IRequest<Result<MenuVariantInventoryDto>>
{
    public GetMenuVariantInventoryQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
