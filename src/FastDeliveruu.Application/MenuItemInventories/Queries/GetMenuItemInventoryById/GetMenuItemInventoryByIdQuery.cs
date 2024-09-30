using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuItemInventories.Queries.GetMenuItemInventoryById;

public class GetMenuItemInventoryByIdQuery : IRequest<Result<MenuItemInventoryDto>>
{
    public GetMenuItemInventoryByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
